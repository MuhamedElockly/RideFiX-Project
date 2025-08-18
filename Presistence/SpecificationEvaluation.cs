
using Domain.Contracts.SpecificationContracts;
using Microsoft.EntityFrameworkCore;

namespace Presistence
{
    static public class SpecificationEvaluation
    {
        static public IQueryable<T> ApplySpecification<T, TK>(this IQueryable<T> query, ISpecification<T, TK> specification) where T : Domain.Entities.BaseEntity<TK>
        {
           
            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }
            foreach (var include in specification.Includes)
            {
                query = query.Include(include);
            }
            foreach (var includeStr in specification.IncludeStrings)
            {
                query = query.Include(includeStr); 
            }
            if (specification.OrderBy != null)
            {
                query = query.OrderBy(specification.OrderBy);
            }
            if (specification.OrderByDescending != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }
            if (specification.IsPagingEnabled)
            {
                query = query.Skip(specification.Skip).Take(specification.Take ?? 10); // Default take to 10 if not specified
            }
            
            if (specification.IsTracking.HasValue)
            {
                query = specification.IsTracking.Value ? query.AsTracking() : query.AsNoTracking();
            }

            return query;
        }
    }
}
