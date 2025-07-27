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

    public OrderSpecifications(string paymentIntentId, bool isPaymentIntent) : base(o => o.PaymentIntentId == paymentIntentId)
    {
        AddInclude(o => o.DeliveryMethod);
        AddInclude(o => o.OrderItems);
    }

    public OrderSpecifications(OrderSpecParams specParams):base(
        x => (string.IsNullOrEmpty(specParams.filter) || x.Status == ParseStatus(specParams.filter)) )
    {
        AddInclude(o => o.DeliveryMethod);
        AddInclude(o => o.OrderItems);

        ApplyPagination(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);

        AddOrderByDesc(o => o.OrderDate);
    }

    public OrderSpecifications( int id)
      : base(o => o.Id == id)
    {
        AddInclude(o => o.DeliveryMethod);
        AddInclude(o => o.OrderItems);

        AddOrderByDesc(o => o.OrderDate);
    }

    private static OrderStatus? ParseStatus(string status)
    {
        if(Enum.TryParse<OrderStatus>(status, true, out var parsedStatus))
            return parsedStatus;
        
        return null;
    }
}