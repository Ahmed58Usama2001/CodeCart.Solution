using CodeCart.Core.Entities;
using System.Linq.Expressions;

namespace CodeCart.Core.Specifications;

public class BaseSpecifications<T> : ISpecification<T> where T : BaseEntity
{
    public Expression<Func<T, bool>> Criteria { get; set; } = null!;
    public Expression<Func<T, object>> OrderBy { get; set; } = null!;
    public Expression<Func<T, object>> OrderByDesc { get; set; } = null!;
    public int Take { get; set; }
    public int Skip { get; set; }
    public bool IsPaginationEnabled { get; set; }

    public BaseSpecifications()
    {
        
    }

    public BaseSpecifications(Expression<Func<T,bool>> criteria)
    {
        Criteria = criteria;
    }

    public void AddOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    public void AddOrderByDesc(Expression<Func<T, object>> orderByDescExpression)
    {
        OrderByDesc = orderByDescExpression;
    }

    public void ApplyPagination(int skip , int take)
    {
        Take = take;
        Skip = skip;
        IsPaginationEnabled = true;
    }
}
