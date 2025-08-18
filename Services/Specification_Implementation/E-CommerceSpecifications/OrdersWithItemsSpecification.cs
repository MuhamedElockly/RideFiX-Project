using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.e_Commerce;
using Services.Specification_Implementation;

namespace Service.Specification_Implementation.E_CommerceSpecifications
{
    public class OrdersWithItemsSpecification : Specification<Order, int>
    {
        public OrdersWithItemsSpecification() : base(x => 1 == 1)
        {
            AddInclude(x => x.orderItems);
            AddInclude("orderItems.Product");

        }

        public OrdersWithItemsSpecification(int orderId) : base(o => o.Id == orderId)
        {
            AddInclude(x => x.orderItems);
            AddInclude("orderItems.Product");
        }
    }
}
