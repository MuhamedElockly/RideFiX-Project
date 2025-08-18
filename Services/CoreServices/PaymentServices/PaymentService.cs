using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;
using Domain.Entities.credit;
using Domain.Entities.IdentityEntities;
using Domain.Entities.PaymentEntites;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Service.CoreServices.E_Commerce;
using Service.Exception_Implementation.ArgumantNullException;
using Service.Specification_Implementation.PaymentSpecification;
using ServiceAbstraction.CoreServicesAbstractions;
using ServiceAbstraction.CoreServicesAbstractions.E_Commerce_Abstraction;
using SharedData.DTOs.Payment;
using Stripe;

namespace Service.CoreServices.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork unitOfWork;
        private IConfiguration configuration;
        private readonly IShoppingCartService shoppingCartService;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentService(IUnitOfWork unitOfWork,
            IConfiguration configuration, 
            IShoppingCartService shoppingCartService,
            UserManager<ApplicationUser> userManager)
        {
            this.unitOfWork = unitOfWork;
            this.configuration = configuration;
            this.shoppingCartService = shoppingCartService;
            _userManager = userManager;
        }

        public async Task<RideCoinsPaymentDto> CreateOrUpdatePaymentIntentRideCoinsAsync(int coins)
        {
            var CoinChargeId = await CreateChargeEntityAsync(coins);
            // Config Stripe
            StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"];
            // Get Coin Top Up
            var coinPaidRequest = await unitOfWork.GetRepository<CoinChargeEntity, int>().GetByIdAsync(CoinChargeId);
            ArgumentNullException.ThrowIfNull(coinPaidRequest);

            // Create or update payment intent
            var PaymentService = new PaymentIntentService();
            if (coinPaidRequest.PaymentIntentId is null)
            {
                var Options = new PaymentIntentCreateOptions()
                {
                    Amount = coinPaidRequest.AmountCents,
                    Currency = "USD",
                    PaymentMethodTypes = ["card"]
                };
                var PaymentIntent = await PaymentService.CreateAsync(Options);
                coinPaidRequest.ClientSecret = PaymentIntent.ClientSecret;
                coinPaidRequest.PaymentIntentId = PaymentIntent.Id;
            }
            else
            {
                var Options = new PaymentIntentUpdateOptions()
                {
                    Amount = coinPaidRequest.AmountCents,
                };
                await PaymentService.UpdateAsync(coinPaidRequest.PaymentIntentId, Options);
            }
            // Save changes
            await unitOfWork.SaveChangesAsync();
            // Return DTO
            return new RideCoinsPaymentDto
            {
                Id = coinPaidRequest.Id,
                Coins = coinPaidRequest.Coins,
                AmountCents = coinPaidRequest.AmountCents,
                ClientSecret = coinPaidRequest.ClientSecret,
                PaymentIntentId = coinPaidRequest.PaymentIntentId
            };
        }

        public async Task<int> CreateChargeEntityAsync(int Coins)
        {
            var repository = unitOfWork.GetRepository<CoinChargeEntity, int>();
            var coinChargeEntity = new CoinChargeEntity
            {
                Coins = Coins,
                AmountCents = Coins * 100, // Assuming 1 coin = 1 USD
                ClientSecret = null,
                PaymentIntentId = null
            };
            await repository.AddAsync(coinChargeEntity);
            await unitOfWork.SaveChangesAsync();
            return coinChargeEntity.Id;
        }

        public async Task CreateTransaction(int CoinChargeId)
        {
            if (CoinChargeId <= 0)
            {
                throw new PaymentArgumentException("Invalid Coin Charge ID");
            }
            var coin = await GetCoinTopUpByChargeEntityId(CoinChargeId);
            if (coin != null)
            {
                throw new PaymentArgumentException("Coin Top Up already exists for this charge entity");
            }
            var coinChargeEntity = await unitOfWork.GetRepository<CoinChargeEntity, int>().GetByIdAsync(CoinChargeId);
            if (coinChargeEntity == null)
            {
                throw new PaymentArgumentException("Coin Charge Entity not found");
            }
            ApplicationUser applicationUser = await GetApplicationUser();
            applicationUser.Coins += coinChargeEntity.Coins;

            var result = await _userManager.UpdateAsync(applicationUser);
            if(!result.Succeeded)
            {
                throw new PaymentArgumentException("can not save in data base");
            }
            var coinTopUp = new CoinTopUp()
            {
                UserId = shoppingCartService.GetUserId(),
                coinChargeEntityId = coinChargeEntity.Id
            };
            await unitOfWork.GetRepository<CoinTopUp, int>().AddAsync(coinTopUp);

            //coinChargeEntity.IsPaid = true;
            await unitOfWork.SaveChangesAsync();

        }

        public async Task<CoinTopUp> GetCoinTopUpByChargeEntityId(int ChargeEntityId)
        {
            if (ChargeEntityId <= 0)
            {
                throw new PaymentArgumentException("Invalid Charge Entity ID");
            }
            var spec = new GetPaymentEntentIdSpecification(ChargeEntityId);
            var coinTopUp = await unitOfWork.GetRepository<CoinTopUp, int>().GetAllAsync(spec);
            if (coinTopUp == null || !coinTopUp.Any())
            {
                return null;
            }
            return coinTopUp.FirstOrDefault();

        }

        public async Task<ApplicationUser> GetApplicationUser()
        {
            string userId = shoppingCartService.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                throw new PaymentArgumentException("User ID cannot be null or empty.");
            }
            var applicationUser = await _userManager.FindByIdAsync(userId);
            if (applicationUser == null)
            {
                throw new PaymentArgumentException("User not found.");
            }
            if (applicationUser.Coins < 0)
            {
                throw new PaymentArgumentException("Personal coins cannot be negative.");
            }
            return applicationUser;

        }

        public async Task<int> GetPersonalCoins()
        {
            var applicationUser = await GetApplicationUser();
            return applicationUser.Coins;
           
        }
    }
}
