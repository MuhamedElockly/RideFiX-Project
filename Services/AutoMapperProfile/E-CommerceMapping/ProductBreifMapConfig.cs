using AutoMapper;
using Domain.Entities.e_Commerce;
using SharedData.DTOs.E_CommerceDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.AutoMapperProfile.E_CommerceMapping
{
    public class ProductBreifMapConfig : Profile
    {
        public ProductBreifMapConfig()
        {
            CreateMap<Product, ProductBreifDTO>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id))
                 .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src =>
                   src.ProductRates.Any() ? src.ProductRates.Average(r => r.Value) : 0))
                .ForMember(dest => dest.TotalRatings, opt => opt.MapFrom(src => src.ProductRates.Count))
                    .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))

                .ReverseMap();
              
        }
    }
}
