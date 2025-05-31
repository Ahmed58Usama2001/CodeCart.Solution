using CodeCart.Core.Entities;
using CodeCart.Core.Services.Contracts;

namespace CodeCart.Service;

public class PaymentService : IPaymentService
{
    public Task<ShoppingCart> CreateOrUpdatePaymentIntent(string cartId)
    {
        throw new NotImplementedException();
    }
}
