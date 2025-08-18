using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Entities.IdentityEntities;
using SharedData.DTOs.Account;

namespace Service.AutoMapperProfile
{
    public class RegisterMapping : Profile
    {
        public RegisterMapping() {

            CreateMap<RegisterStep1Dto, ApplicationUser>()
           .ForMember(d => d.UserName, s => s.MapFrom(s => s.Email));
           


        }
    }
}
