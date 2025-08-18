using Domain.Entities.credit;
using Services.Specification_Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specification_Implementation.PaymentSpecification
{
    public class GetPaymentEntentIdSpecification : Specification<CoinTopUp, int>
    {
        public GetPaymentEntentIdSpecification(int id) : base(s => s.coinChargeEntityId == id)
        {
        }
    }
}