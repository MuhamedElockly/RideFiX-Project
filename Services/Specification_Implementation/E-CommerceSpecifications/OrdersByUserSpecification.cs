using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.e_Commerce;
using Services.Specification_Implementation;

namespace Service.Specification_Implementation.E_CommerceSpecifications
{
    public class OrdersByUserSpecification : Specification<Order, int>
    {
        public OrdersByUserSpecification(string userId) : base(o => o.UserId == userId)
        {
            AddInclude(x => x.orderItems);
            AddInclude("orderItems.Product");
        }
    }
}
