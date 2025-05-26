using CodeCart.Core.Entities;
using CodeCart.Core.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace CodeCart.API.Controllers;


public class CartController(ICartService cartService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<ShoppingCart>> GetCartById(string id)
    {
        var cart =await cartService.GetCartAsync(id);

        return Ok(cart ?? new ShoppingCart { Id = id });
    }

    [HttpPost]
    public async Task<ActionResult<ShoppingCart>> UpdateCart(ShoppingCart cart)
    {
        var updatedCart =await cartService.SetCartAsync(cart);

        if (updatedCart == null) return BadRequest("Problem with Cart");

        return Ok(updatedCart);
    }

    [HttpDelete]
    public async Task<ActionResult>DeleteCart(string id)
    {
        var result = await cartService.DeleteCartAsync(id);

        if (!result)
            return BadRequest("Problem deleting cart");

        return NoContent();
    }
}
