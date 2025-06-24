using CodeCart.Core.Entities;
using CodeCart.Core.Entities.OrderAggregation;
using System.ComponentModel.DataAnnotations;

namespace CodeCart.API.DTOs;

public class CreateOrderDto
{
    [Required]
    public string CartId { get; set; }= string.Empty;

    [Required]
    public int DeliveryMethodId { get; set; }

    [Required]
    public ShippingAddress ShippingAddress { get; set; } = null!;

    [Required]
    public PaymentSummary PaymentSummary { get; set; } = null!;

}

public class OrderToReturnDto
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public required string BuyerEmail { get; set; }
    public required ShippingAddress ShippingAddress { get; set; } 
    public required string DeliveryMethod { get; set; }
    public  decimal ShippingPrice { get; set; }
    public required PaymentSummary PaymentSummary { get; set; }
    public List<OrderItemDto> OrderItems { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Total { get; set; }
    public required string OrderStatus { get; set; } 
    public required string PaymentIntentId { get; set; }
}
