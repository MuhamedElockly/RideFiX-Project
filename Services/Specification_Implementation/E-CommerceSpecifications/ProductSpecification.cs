using Domain.Entities.e_Commerce;
using Services.Specification_Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specification_Implementation.E_CommerceSpecifications
{
    public class ProductSpecification : Specification<Product, int>
    {
        public ProductSpecification(int ProductId) : base(s => s.Id == ProductId)
        {
            AddInclude(x => x.ProductRates);
            AddInclude("ProductRates.ApplicationUser");


        }
    }
}
