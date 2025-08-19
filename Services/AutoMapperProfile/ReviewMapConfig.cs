using AutoMapper;
using Domain.Entities.CoreEntites.EmergencyEntities;
using SharedData.DTOs.ReviewsDTOs;
using SharedData.DTOs.TechnicianDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.AutoMapperProfile
{
    public class ReviewMapConfig : Profile
    {
        public ReviewMapConfig()
        {
            CreateMap<Review, ReviewDTO>();
            CreateMap<Review, ReadTechnicianReviewDTO>()
                .ForMember(dest => dest.CarOwnerName, opt => opt.MapFrom(src => src.CarOwner.ApplicationUser.Name))
                .ForMember(dest => dest.TechnicianName, opt => opt.MapFrom(src => src.Technician.ApplicationUser.Name));



        }
    }
}
