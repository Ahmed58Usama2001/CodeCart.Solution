namespace CodeCart.Core.Entities.OrderAggregation;

public class PaymentSummary
{
    public int Last4 { get; set; }
    public required string Brand { get; set; }
    public int ExpMonth { get; set; }
    public int Year { get; set; }
}
