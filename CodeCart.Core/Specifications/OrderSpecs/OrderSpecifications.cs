using CodeCart.Core.Entities.OrderAggregation;
using System.Linq.Expressions;

namespace CodeCart.Core.Specifications.OrderSpecs;

public class OrderSpecifications : BaseSpecifications<Order>
{
    public OrderSpecifications(string email)
        : base(o=>o.BuyerEmail ==email)
    {
        AddInclude(o => o.DeliveryMethod);
        AddInclude(o => o.OrderItems);

        AddOrderByDesc(o => o.OrderDate);
    }

    public OrderSpecifications(string email, int id)
       : base(o => o.BuyerEmail == email && o.Id == id)
    {
        AddInclude(o => o.DeliveryMethod);
        AddInclude(o => o.OrderItems);

        AddOrderByDesc(o => o.OrderDate);
    }
}