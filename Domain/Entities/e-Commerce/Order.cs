using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedData.Enums;

namespace Domain.Entities.e_Commerce
{
    public class Order : BaseEntity<int>
    {
        public string location { get; set; }
        public OrderStatus orderState { get; set; }
        public decimal totalPrice { get; set; }
        public string UserId { get; set; }


        public ICollection<OrderItem> orderItems { get; set; } = new HashSet<OrderItem>();

    }
}
