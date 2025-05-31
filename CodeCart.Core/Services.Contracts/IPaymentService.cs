using CodeCart.Core.Entities;

namespace CodeCart.Core.Services.Contracts;

public interface IPaymentService
{
    Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId);
}
