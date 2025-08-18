using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Domain.Entities.e_Commerce;
using SharedData.DTOs.E_CommerceDTOs;

namespace Service.AutoMapperProfile.E_CommerceMapping
{
    public class ProductWithRatesMapConfig : Profile
    {
        public ProductWithRatesMapConfig()
        {
            CreateMap<Product, ProductWithRatesDTO>()
                .IncludeBase<Product, ProductBreifDTO>() 
                .ReverseMap();
        }
    }

}
