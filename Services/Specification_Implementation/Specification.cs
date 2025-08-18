using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts.SpecificationContracts;
using Domain.Entities;
using Service.Specification_Implementation;

namespace Services.Specification_Implementation
{
    public abstract class Specification<T, TK> : ISpecification<T, TK> where T : BaseEntity<TK>
    {
        #region Includes
        public Expression<Func<T, bool>> Criteria { get; private set; }
        public List<string> IncludeStrings { get; } = new List<string>();

        public Specification(Expression<Func<T, bool>> _criteria)
        {
            Criteria = _criteria;
        }

        public Specification()
        {
            Criteria = null;
        }


        public List<Expression<Func<T, object>>> Includes { get; } = [];


        public void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        public void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }
        #endregion

        #region Ordering
        public Expression<Func<T, object>> OrderBy { get; private set; }

        public Expression<Func<T, object>> OrderByDescending { get; private set; }

        public void SetOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }
        public void SetOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            OrderByDescending = orderByDescendingExpression;
        }
        #endregion

        #region Pagination
        public int? Take { get; private set; }
        public int Skip { get; private set; }
        public bool IsPagingEnabled { get; set; }
        public void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }

        public void AddCriteria(Expression<Func<T, bool>> criteriaExpression)
        {
            if (Criteria == null)
            {
                Criteria = criteriaExpression;
            }
            else
            {
                Criteria = Criteria.AndAlso(criteriaExpression);
            }
        }
        #endregion

        public bool? IsTracking { get; set; }

    }
}
