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
    public class FilterProductSpecification : Specification<Product, int>
    {
        public FilterProductSpecification(
            int? pageNumber,
            int? itemPerPage,
            decimal? maxPrice = null,
            int? categoryId = null)
        {
            if (maxPrice.HasValue)
            {
                AddCriteria(s => s.Price <= maxPrice.Value);
            }

            if (categoryId.HasValue)
            {
                AddCriteria(s => s.CategoryId == categoryId.Value);
            }

            AddInclude(s => s.Category);
            AddInclude(s => s.ProductRates);

            SetOrderBy(s => s.Price);

            if (pageNumber.HasValue)
            {
                int skip = (pageNumber.Value - 1) * (itemPerPage ?? 10);
                int take = itemPerPage ?? 10;
                ApplyPaging(skip, take);
            }
            
            else if (itemPerPage.HasValue)
            {
                ApplyPaging(0, itemPerPage.Value);
            }
          
            else
            {
                ApplyPaging(0, 10);
            }
        }
    }
}
