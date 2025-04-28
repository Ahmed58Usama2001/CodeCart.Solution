using CodeCart.Core.Entities;
using System.Linq.Expressions;

namespace CodeCart.Core.Specifications;

public interface ISpecefication<T> where T : BaseEntity
{
    public Expression<Func<T,bool>> Criteria { get; set; }

    public Expression<Func<T,object>> OrderBy { get; set; }

    public Expression<Func<T, object>> OrderByDesc { get; set; }

   
}
