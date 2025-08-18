using AutoMapper;
using Domain.Contracts;
using Domain.Entities.e_Commerce;
using Service.Exception_Implementation.ArgumantNullException;
using Service.Specification_Implementation.E_CommerceSpecifications;
using ServiceAbstraction.CoreServicesAbstractions.E_Commerce_Abstraction;
using SharedData.DTOs.E_CommerceDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.CoreServices.E_Commerce
{
    public class ProductsService : IProductsService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public ProductsService(IUnitOfWork _unitOfWork, IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }
        public async Task<List<ProductBreifDTO>> FilterProductsAsync(
            int? pageNumber,
            int? itemPerPage,
            decimal? maxPrice = null,
            int? categoryId = null)
        {
            var spec = new FilterProductSpecification( pageNumber,
             itemPerPage, maxPrice, categoryId);
            var products = await unitOfWork.GetRepository<Product, int>().GetAllAsync(spec);
            if (products == null || !products.Any())
            {
                return new List<ProductBreifDTO>();
            }
            var productsDto = mapper.Map<List<ProductBreifDTO>>(products);
            return productsDto;
        }

        public async Task<CartItemDto> GetProductByIdAsync(int productId)
        {
            if(productId <= 0)
            {
                throw new ProductArgumentException("Product ID must be greater than zero.");
            }
            var spec = new ProductSpecification(productId);
            var Product = await unitOfWork.GetRepository<Product, int>().GetByIdAsync(spec);
            if (Product == null)
            {
                throw new ProductArgumentException($"Product with ID {productId} not found.");
            }
            var productDto = mapper.Map<CartItemDto>(Product);
            return productDto;
        }

        //public async Task<ProductWithRatesDTO> GetProductDetailsByIdAsync(int productId)
        //{
        //    if (productId <= 0)
        //    {
        //        throw new ProductArgumentException("Product ID must be greater than zero.");
        //    }
        //    var spec = new ProductSpecification(productId);
        //    var Product = await unitOfWork.GetRepository<Product, int>().GetByIdAsync(spec);
        //    if (Product == null)
        //    {
        //        throw new ProductArgumentException($"Product with ID {productId} not found.");
        //    }
        //    var productDto = mapper.Map<ProductWithRatesDTO>(Product);
        //    return productDto;
        //}

        public async Task<ProductWithRatesDTO> GetProductDetailsByIdAsync(int productId)
        {
            if (productId <= 0)
            {
                throw new ProductArgumentException("Product ID must be greater than zero.");
            }

            var spec = new ProductSpecification(productId);
            var product = await unitOfWork.GetRepository<Product, int>().GetByIdAsync(spec);
            Console.WriteLine(product.Id);
            if (product == null)
            {
                throw new ProductArgumentException($"Product with ID {productId} not found.");
            }

            var productDto = mapper.Map<ProductWithRatesDTO>(product);

            if (product.ProductRates != null && product.ProductRates.Any())
            {
                productDto.TotalRatings = product.ProductRates.Count;
                productDto.AverageRating = product.ProductRates.Average(r => r.Value);

                productDto.ProductRates = product.ProductRates.Select(r => new RateDTO
                {
                    Value = r.Value,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    UserName = r.ApplicationUser.Name,
                    UserId = r.ApplicationUser.Id

                }).ToList();
            }

            return productDto;
        }













        public async Task<List<ProductBreifDTO>> ProductSearchByName(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
            {
                throw new ProductArgumentException("Product name cannot be null or empty.");
            }
            var spec = new ProductSearchSpecification(productName);
            var products = await unitOfWork.GetRepository<Product, int>().GetAllAsync(spec);
            if (products == null || !products.Any())
            {
                return new List<ProductBreifDTO>();
            }
            var productsDto = mapper.Map<List<ProductBreifDTO>>(products);
            return productsDto;
        }


    }
}
