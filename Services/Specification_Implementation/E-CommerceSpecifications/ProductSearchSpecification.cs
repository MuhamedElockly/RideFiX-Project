using Domain.Entities.e_Commerce;
using Services.Specification_Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specification_Implementation.E_CommerceSpecifications
{
    public class ProductSearchSpecification : Specification<Product, int>
    {
        public ProductSearchSpecification(string productName)
            : base(p => p.Name.ToUpper().Contains(productName.Trim().ToUpper()))
        {
            AddInclude(p => p.ProductRates);

        }
    }
  
    }
