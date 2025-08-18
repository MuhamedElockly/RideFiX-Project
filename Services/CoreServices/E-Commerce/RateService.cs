using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Domain.Contracts;
using Domain.Entities.e_Commerce;
using ServiceAbstraction.CoreServicesAbstractions.E_Commerce_Abstraction;

namespace Service.CoreServices.E_Commerce
{
    public class RateService : IRateService
    {
        private readonly IUnitOfWork unitOfWork;

        public RateService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task AddRateAsync(int productId, int value, string comment, string userId)
        {
            if (value < 1 || value > 5)
                throw new ArgumentException("Rate must be between 1 and 5.");

            var product = await unitOfWork.GetRepository<Product, int>().GetByIdAsync(productId);
            if (product == null)
                throw new ArgumentException($"Product with ID {productId} not found.");

            var rateRepo = unitOfWork.GetRepository<Rate, int>();
            var existingRate = await rateRepo
                .GetFirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == userId);

            if (existingRate != null)
            {
                existingRate.Value = value;
                existingRate.Comment = comment;
                rateRepo.Update(existingRate);
            }
            else
            {
                var rate = new Rate
                {
                    ProductId = productId,
                    UserId = userId,
                    Value = value,
                    Comment = comment
                };
                await rateRepo.AddAsync(rate);
            }
            await unitOfWork.SaveChangesAsync();
        }
    }


}
