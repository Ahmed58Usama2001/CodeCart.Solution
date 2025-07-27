using CodeCart.Core.Entities;
using CodeCart.Core.Entities.OrderAggregation;
using CodeCart.Core.Specifications.ProductSpecs;
using System.Linq.Expressions;

namespace CodeCart.Core.Specifications.OrderSpecs;

public class OrderForCountSpecifications : BaseSpecifications<Order>
{
    public OrderForCountSpecifications(OrderSpecParams specParams)
        : base(GetCriteria(specParams))
    {
    }




    private static Expression<Func<Order, bool>> GetCriteria(OrderSpecParams specParams)
    {
        return x => (string.IsNullOrEmpty(specParams.filter) || x.Status == ParseStatus(specParams.filter));
    }

    private static OrderStatus? ParseStatus(string status)
    {
        if (Enum.TryParse<OrderStatus>(status, true, out var parsedStatus))
            return parsedStatus;

        return null;
    }
}
