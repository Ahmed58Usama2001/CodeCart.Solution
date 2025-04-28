using CodeCart.Core.Entities;
using CodeCart.Core.Specifications;

namespace CodeCart.Infrastructure;

public static class SpecificationsEvaluator<T> where T : BaseEntity
{
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery , ISpecefications<T> specs)
    {
        var query = inputQuery;

        if(specs.Criteria is not null)
            query = query.Where(specs.Criteria);

        if (specs.OrderBy is not null)
            query = query.OrderBy(specs.OrderBy);
        else if (specs.OrderByDesc is not null)
            query = query.OrderByDescending(specs.OrderByDesc);

        return query;
    }
}
