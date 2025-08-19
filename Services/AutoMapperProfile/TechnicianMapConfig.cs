using AutoMapper;
using Domain.Entities.CoreEntites.EmergencyEntities;
using SharedData.DTOs.Admin.Users;
using SharedData.DTOs.TechnicianDTOs;

namespace Service.AutoMapperProfile
{
    public class TechnicianMapConfig : Profile
    {
        public TechnicianMapConfig()
        {
            CreateMap<Technician, TechnicianDTO>()
              .ForMember(dest => dest.FaceImageUrl, opt => opt.MapFrom(src => src.ApplicationUser.FaceImageUrl))
              .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.Id))
              .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.TCategories))
              .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.reviews))
              .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ApplicationUser.Name))
              .ForMember(dest => dest.government, opt => opt.MapFrom(src => src.government.ToString()))
              .ReverseMap();

            CreateMap<Technician, ReadUsersDTO>()
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
             .ForMember(dest => dest.IsActivated, opt => opt.MapFrom(src => src.ApplicationUser.IsActivated))
             .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ApplicationUser.Name))
             .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.ApplicationUser.Email))
             .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.ApplicationUser.PhoneNumber))
             .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.ApplicationUser.FaceImageUrl))
             .ForMember(dest => dest.Rate, opt => opt.MapFrom(src =>
                src.reviews.Any()
                ? (int)Math.Round(src.reviews.Average(r => r.Rate))
                : 0
             ));
          
         


        }
    }
}
