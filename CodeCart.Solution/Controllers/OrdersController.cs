using CodeCart.API.DTOs;
using CodeCart.Core.Entities;
using CodeCart.Core.Entities.OrderAggregation;
using CodeCart.Core.Repositories.Contracts;
using CodeCart.Core.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CodeCart.API.Controllers;

[Authorize]
public class OrdersController(ICartService cartService, IUnitOfWork unitOfWork) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<Order>> CreateOrder(CreateOrderDto createOrderDto)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);

        if (string.IsNullOrEmpty(email))
            return BadRequest("User email not found");

        var cart = await cartService.GetCartAsync(createOrderDto.CartId);
        if (cart == null) return BadRequest("Cart not found");
        if (cart.PaymentIntentId == null) return BadRequest("No Payment Intent for this order");

        var items = new List<OrderItem>();
        foreach (var item in cart.Items)
        {
            var productItem = await unitOfWork.Repository<Product>().GetByIdAsync(item.ProductId);
            if (productItem == null) return BadRequest("Problem with the order");

            var itemOrdered = new ProductItemOrdered
            {
                PictureUrl = item.PictureUrl,
                ProductName = item.ProductName,
                ProductId = item.ProductId
            };

            var orderItem = new OrderItem
            {
                ItemOrdered = itemOrdered,
                Price = productItem.Price,
                Quantity = item.Quantity,
            };
            items.Add(orderItem);
        }

        var deliveryMethod = await unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(createOrderDto.DeliveryMethodId);
        if (deliveryMethod == null) return BadRequest("No delivery method selected");

        var order = new Order
        {
            BuyerEmail = email,
            ShippingAddress = createOrderDto.ShippingAddress,
            PaymentSummary = createOrderDto.PaymentSummary,
            PaymentIntentId = cart.PaymentIntentId,
            OrderItems = items,
            DeliveryMethod = deliveryMethod,
            Subtotal = items.Sum(i => i.Quantity * i.Price)
        };

        await unitOfWork.Repository<Order>().CreateAsync(order);

        if (await unitOfWork.CompleteAsync() > 0)
            return order;
        else
            return BadRequest("Problem creating the order");
    }
}