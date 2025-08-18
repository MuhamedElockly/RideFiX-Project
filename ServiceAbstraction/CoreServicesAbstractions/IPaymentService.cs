using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.credit;
using Domain.Entities.IdentityEntities;
using SharedData.DTOs.Payment;

namespace ServiceAbstraction.CoreServicesAbstractions
{
    public interface IPaymentService
    {
        public Task<RideCoinsPaymentDto> CreateOrUpdatePaymentIntentRideCoinsAsync(int CoinChargeId);
        public Task<int> CreateChargeEntityAsync(int Coins);
        public Task CreateTransaction(int CoinChargeId);
        public Task<CoinTopUp> GetCoinTopUpByChargeEntityId(int ChargeEntityId);

        public Task<ApplicationUser> GetApplicationUser();
        public Task<int> GetPersonalCoins();
    }
}
