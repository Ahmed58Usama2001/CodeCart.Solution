using CodeCart.Core.Entities;
using CodeCart.Core.Repositories.Contracts;
using CodeCart.Core.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeCart.API.Controllers;

public class PaymentsController(IPaymentService paymentService,
    IGenericRepository<DeliveryMethod> dmMethodsRepo) : BaseApiController
{
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
        return Ok(await dmMethodsRepo.GetAllAsync());
    }
}