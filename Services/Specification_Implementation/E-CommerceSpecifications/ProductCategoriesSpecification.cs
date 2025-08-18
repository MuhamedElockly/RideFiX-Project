using Domain.Entities.e_Commerce;
using Services.Specification_Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.Specification_Implementation.E_CommerceSpecifications
{
    public class ProductCategoriesSpecification : Specification<Category, int>
    {
        public ProductCategoriesSpecification() : base(x=> 1 == 1)
        {
            AddInclude(x => x.Products);

        }
        public ProductCategoriesSpecification(int CategoryID) : base(s => s.Id == CategoryID)
        {
            AddInclude(x => x.Products);
            AddInclude(x => x.Products.Select(p => p.Category.Name));


        }
    }
}
