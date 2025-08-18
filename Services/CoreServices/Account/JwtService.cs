using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;
using Domain.Entities.CoreEntites.EmergencyEntities;
using Domain.Entities.IdentityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServiceAbstraction.CoreServicesAbstractions.Account;
using SharedData.DTOs.Account;

namespace Service.CoreServices.Account
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        public string generateToken( JwtTokenDto user, IList<string> roles  ,int roleEntityId)
        {
            var claims = new List<Claim>() {
                new Claim("userId" , user.Id),
                new Claim("Id", roleEntityId.ToString()),
                new Claim("Email" , user.Email) , 
                new Claim ("Name" , user.Name) ,
                new Claim("ProfilePicUrl", user.ProfilePic?? ""),
            };
            foreach(var role in roles)
            claims.Add(new Claim("Role", role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(

                    issuer: _configuration["JWT:Issuer"],
                    audience: _configuration["JWT:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddDays(2),
                    signingCredentials: creds
                );
            var stringToken = new JwtSecurityTokenHandler().WriteToken(token);

            return stringToken.ToString();
        }

       
    }
}
