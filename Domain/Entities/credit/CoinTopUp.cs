using Domain.Entities.PaymentEntites;
using SharedData.DTOs.Credit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.credit
{
    public class CoinTopUp : BaseEntity<int>
    {
        public string UserId { get; set; }


        [ForeignKey("coinChargeEntity")]

        public int coinChargeEntityId { get; set; }
        public CoinChargeEntity coinChargeEntity { get; set; }


    }
}
