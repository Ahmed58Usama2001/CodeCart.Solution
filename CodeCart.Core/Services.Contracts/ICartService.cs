using CodeCart.Core.Entities;

namespace CodeCart.Core.Services.Contracts;

public interface ICartService
{
    Task<ShoppingCart?> GetCartAsync(string key);

    Task<ShoppingCart?> SetCartAsync(ShoppingCart cart);

    Task<bool> DeleteCartAsync(string key);
}
