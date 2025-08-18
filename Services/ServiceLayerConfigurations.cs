using Microsoft.Extensions.DependencyInjection;
using Service.AutoMapperProfile;
using Service.CoreServices;
using Service.CoreServices.Account;
using Service.CoreServices.Admin;
using Service.CoreServices.CarMservices;
using Service.CoreServices.ChatServices;
using Service.CoreServices.E_Commerce;
using Service.CoreServices.EmergencyReqServices;
using Service.CoreServices.PaymentService;
using Service.CoreServices.ReportsServices;
using ServiceAbstraction;
using ServiceAbstraction.CoreServicesAbstractions;
using ServiceAbstraction.CoreServicesAbstractions.Account;
using ServiceAbstraction.CoreServicesAbstractions.Admin;
using ServiceAbstraction.CoreServicesAbstractions.CarMservices;
using ServiceAbstraction.CoreServicesAbstractions.E_Commerce_Abstraction;
using ServiceAbstraction.CoreServicesAbstractions.Reports;



namespace Services
{
    public static class ServiceLayerConfigurations
    {
        public static IServiceCollection AddServiceConfig(this IServiceCollection Services)
        {
            Services.AddHttpContextAccessor();
            Services.AddScoped<IServiceManager, ServiceManager>();
            Services.AddAutoMapper(typeof(RegisterMapping));
            Services.AddScoped<IFileService, FileService>();

            Services.AddHttpClient(); // تحضير الـ HttpClient في الـ DI
            Services.AddScoped<IFaceRecognitionService, FaceRecognitionService>(); Services.AddScoped<IAuthService, AuthService>();
            Services.AddMemoryCache();
            Services.AddScoped<IJwtService, JwtService>();

            Services.AddScoped<IRequestServices, RequstServices>();
            Services.AddScoped<ITechnicianService, TechnicianService>();
            Services.AddScoped<ITechnicianRequestEmergency, TechnicianRequestEmergency>();
            Services.AddScoped<ICategoryService, CategoryService>();
            Services.AddScoped<IReviewService, ReviewService>();
            Services.AddScoped<IUserProfileService, UserProfileService>();
            Services.AddAutoMapper(typeof(UserProfileMapConfig));
            Services.AddAutoMapper(typeof(PreRequestMapConfig));
            Services.AddScoped<ICarOwnerService, CarOwnerService>();
            Services.AddScoped<IMessegeService, MessegeService>();
            Services.AddScoped<IChatService, ChatService>();
            Services.AddScoped<IUserConnectionIdService, UserConnectionIdService>();
            Services.AddScoped<IChatSessionService, ChatSessionService>();
            Services.AddScoped<ICarServices, CarServices>();
            Services.AddScoped<ICarMaintananceService, CarMaintananceService>();
            Services.AddScoped<IMaintenanceTypesService, MaintenanceTypesService>();
            Services.AddScoped<IEmailService, EmailService>();
            Services.AddScoped<IReverserRequestService, ReverserRequestService>();
            Services.AddScoped<IProductCategoryService, ProductCategoryService>();
            Services.AddScoped<IProductsService, ProductsService>();
            Services.AddScoped<IShoppingCartService, ShoppingCartService>();

            Services.AddScoped<IOrderService , OrderService>();
            Services.AddScoped<IRateService, RateService>();

            Services.AddScoped<IReportsServices, ReportsServices>();

            Services.AddScoped<IAdminService, AdminService>();
            Services.AddScoped<IActivityReportService, ActivityReportService>();
            Services.AddScoped<IPaymentService, PaymentService>();


            return Services;
        }
    }
}
