using CodeCart.Core.Entities;
using System.Linq.Expressions;

namespace CodeCart.Core.Specifications;

public interface ISpecification<T> where T : BaseEntity
{
    public Expression<Func<T,bool>> Criteria { get; set; }

    public Expression<Func<T,object>> OrderBy { get; set; }

    public Expression<Func<T, object>> OrderByDesc { get; set; }

    public int Take { get; set; }
    public int Skip { get; set; }
    public bool IsPaginationEnabled { get; set; }

}
