using CodeCart.Core.Entities;
using System.Linq.Expressions;

namespace CodeCart.Core.Specifications;

public class BaseSpecifications<T> : ISpecefications<T> where T : BaseEntity
{
    public Expression<Func<T, bool>> Criteria { get; set; } = null!;
    public Expression<Func<T, object>> OrderBy { get; set; } = null!;
    public Expression<Func<T, bool>> OrderByDesc { get; set; } = null!;

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
        OrderBy = orderByDescExpression;
    }
}
