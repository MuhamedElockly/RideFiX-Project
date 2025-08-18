using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.DTOs.Payment
{
    public class RideCoinsPaymentDto
    {
        public int Id { get; set; }
        public int Coins { get; set; }
        public long AmountCents { get; set; }
        public string? ClientSecret { get; set; }
        public string? PaymentIntentId { get; set; }
    }
}
