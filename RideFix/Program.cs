
using Domain.Contracts;
using Domain.Entities.CoreEntites.CarMaintenance_Entities;
using Domain.Entities.CoreEntites.EmergencyEntities;
using Domain.Entities.IdentityEntities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Presentation.Hubs;
using Presistence;
using Presistence.Data;
using RideFix.CustomMiddlewares;
using Services;
using SharedData.Enums;
using Hangfire;
using Hangfire.MemoryStorage;
using StackExchange.Redis;
using System.Text;

namespace RideFix
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularOrigin",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:4200")  // السماح فقط لواجهة Angular
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .AllowCredentials();  // السماح بالـ credentials (مثل الكوكيز أو التوكنات)
                    });
            });


            builder.Services.AddControllers()
    .AddApplicationPart(typeof(Presentation.Controllers.CarController).Assembly);

            //builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //Redis
            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var configuration = ConfigurationOptions.Parse("localhost:6379,abortConnect=false");
                return ConnectionMultiplexer.Connect(configuration);
            });


            builder.Services.AddSignalR(); // Add SignalR services


            #region Services Configurations
            builder.Services.AddPresistenceConfig(builder.Configuration); // Custom extension method to add persistence layer configurations
            builder.Services.AddServiceConfig();// Custom extension method to add service layer configurations
            #endregion

            #region Authentication And Security
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                            .AddEntityFrameworkStores<ApplicationDbContext>()
                            .AddDefaultTokenProviders();
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
            });


            ; builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });
            //.AddJwtBearer(options =>
            //            {
            //                var config = builder.Configuration;
            //                options.TokenValidationParameters = new TokenValidationParameters
            //                {
            //                    ValidateIssuer = true,
            //                    ValidateAudience = true,
            //                    ValidateLifetime = true,
            //                    ValidateIssuerSigningKey = true,
            //                    ValidIssuer = config["JWT:Issuer"],
            //                    ValidAudience = config["JWT:Audience"],
            //                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]))
            //                };
            //            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
        .AddJwtBearer(options =>
        {
            var config = builder.Configuration;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config["JWT:Issuer"],
                ValidAudience = config["JWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]))
            };

            options.Events = new JwtBearerEvents
            {
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    var result = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        statusCode = 401,
                        message = "Unauthorized: Token is missing or invalid"
                    });
                    return context.Response.WriteAsync(result);
                },
                OnForbidden = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    var result = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        statusCode = 403,
                        message = "Forbidden: You are not allowed to access this resource"
                    });
                    return context.Response.WriteAsync(result);
                },
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken)
                    && path.StartsWithSegments("/requestWatchDogHub") || path.StartsWithSegments("/chathub")
                    || path.StartsWithSegments("/notificationhub"))
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }
            };

        });
            #endregion

            #region Invalid Model State Response Factory Configuration
            builder.Services.Configure<ApiBehaviorOptions>(ApiBehaviorOptions =>
            {
                ApiBehaviorOptions.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .Select(e => new ErrorModels.ValidationError
                        {
                            Key = e.Key,
                            Errors = e.Value.Errors.Select(x => x.ErrorMessage).ToArray()
                        }).ToArray();
                    var Error = new ErrorModels.ValidationErrorToReturn
                    {
                        Errors = errors,
                    };
                    return new BadRequestObjectResult(Error);
                };
            });
            #endregion

            #region Send EMAIL
            builder.Services.AddHangfire(x => x.UseMemoryStorage());
            builder.Services.AddHangfireServer();
            #endregion
            var app = builder.Build();
            app.UseCors("AllowAngularOrigin");


            //notification hub configuration
            // التسجيل الصحيح للـ Hub
            app.MapHub<ChatHub>("/chathub");//http://localhost:5038/chathub
            app.MapHub<RequestWatchDogHub>(pattern: "/requestWatchDogHub");
            app.MapHub<NotificationHub>(pattern: "/notificationhub");




            #region Exception Handler Middleware Configuration
            app.UseMiddleware<CustomExceptionMiddleware>();
            #endregion

            #region Data Seeding Configuration
            using (var scope = app.Services.CreateScope())
            {
                var dataSeeding = scope.ServiceProvider.GetRequiredService<IDataSeeding>();
                await dataSeeding.SeedCategories();
                await dataSeeding.SeedIdentityDataAsync();
                await dataSeeding.SeedDataAsync();
            }
            #endregion


            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthentication();
            app.UseAuthorization();

            //EMAIL
            app.UseHangfireDashboard();
            app.UseStaticFiles();

            app.MapControllers();

            app.Run();

        }
    }
}
