using CodeCart.Core.Entities;
using System.Linq.Expressions;

namespace CodeCart.Core.Specifications;

public interface ISpecefications<T> where T : BaseEntity
{
    public Expression<Func<T,bool>> Criteria { get; set; }
}
