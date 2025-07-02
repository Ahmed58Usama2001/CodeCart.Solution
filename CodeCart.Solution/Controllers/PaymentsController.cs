using AutoMapper;
using CodeCart.API.DTOs;
using CodeCart.API.SignalR;
using CodeCart.Core.Entities;
using CodeCart.Core.Entities.OrderAggregation;
using CodeCart.Core.Repositories.Contracts;
using CodeCart.Core.Services.Contracts;
using CodeCart.Core.Specifications.OrderSpecs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Stripe;

namespace CodeCart.API.Controllers;

public class PaymentsController(IPaymentService paymentService,
    IUnitOfWork unitOfWork,
    ILogger<PaymentsController> logger,
    IConfiguration configuration,
    IHubContext<NotificationHub> hubContext,
    IMapper mapper) : BaseApiController
{
    private readonly string _whSecret = configuration["StripeSettings:WhSecret"]!;

    [Authorize]
    [HttpPost("{cartId}")]
    public async Task<ActionResult<ShoppingCart>> CreateOrUpdatePaymentIntent(string cartId)
    {
        var cart = await paymentService.CreateOrUpdatePaymentIntent(cartId);

        if (cart == null)
            return BadRequest("Problem with your cart");

        return Ok(cart);
    }

    [HttpGet("delivery-methods")]
    public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveyMethods()
    {
        return Ok(await unitOfWork.Repository<DeliveryMethod>().GetAllAsync());
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        try
        {
            var stripeEvent = ConstructStripeEvent(json);
            if (stripeEvent.Data.Object is not PaymentIntent intent)
            {
                return BadRequest("Invalid event data");
            }

            await HandlePaymentIntentSucceded(intent);

            return Ok();
        }
        catch (StripeException ex)
        {
            logger.LogError(ex, "Error processing Stripe webhook");
            return StatusCode(StatusCodes.Status500InternalServerError, "Error processing Stripe webhook");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Something went wrong");
            return StatusCode(StatusCodes.Status500InternalServerError, "Something went wrong");
        }
    }

    private async Task HandlePaymentIntentSucceded(PaymentIntent intent)
    {
        if(intent.Status == "succeeded")
        {
            var spec = new OrderSpecifications(intent.Id, true);
            var order = await unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);
            if (order == null)
            {
                logger.LogWarning("Order not found for PaymentIntentId: {IntentId}", intent.Id);
                return;
            }

            if ((long)(order.Subtotal+order.DeliveryMethod.Price) * 100 != intent.Amount)
            {
                order.Status = OrderStatus.PaymentMismatch;
            }
            else
            {
                order.Status = OrderStatus.PaymentReceived;
            }

            await unitOfWork.CompleteAsync();

            var connectionId = NotificationHub.GetConnectiodIdByEmail(order.BuyerEmail);

            if (connectionId is null)        
                logger.LogWarning("No connection found for email: {Email}", order.BuyerEmail);

            await hubContext.Clients.Client(connectionId!).SendAsync("OrderCompletionNotification" , mapper.Map<Order , OrderToReturnDto>(order));

        }

    }

    private Event ConstructStripeEvent(string json)
    {
        try
        {
           return EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                _whSecret);
        }   
        catch (Exception ex)
        {
            logger.LogError(ex, "Error constructing Stripe event");
            throw new StripeException("Failed to construct Stripe event", ex);
        }
    }
}