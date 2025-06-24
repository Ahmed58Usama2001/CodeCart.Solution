namespace CodeCart.Core.Entities.OrderAggregation;

public class ProductItemOrdered
{
    public int ProductId { get; set; }
    public required string ProductName { get; set; }
    public required string PictureUrl { get; set; }
}
