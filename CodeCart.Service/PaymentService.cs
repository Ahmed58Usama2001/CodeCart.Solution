using CodeCart.Core.Entities;
using CodeCart.Core.Repositories.Contracts;
using CodeCart.Core.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace CodeCart.Service;

public class PaymentService(IConfiguration configuration,
        CartService cartService,
        IGenericRepository<Core.Entities.Product> productRepo,
        IGenericRepository<DeliveryMethod> deliveryMethodsRepo) : IPaymentService
{
    public async Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId)
    {
        StripeConfiguration.ApiKey = configuration["StripeSettings:SecretKey"] ?? throw new InvalidOperationException("Stripe secret key is not configured.");

        var cart = await cartService.GetCartAsync(cartId);
        if (cart == null) return null;

        var shippingPrice = 00m;

        if (cart.DeliveryMethodId.HasValue)
        {
            var deliveryMethod = await deliveryMethodsRepo.GetByIdAsync(cart.DeliveryMethodId.Value);
            if (deliveryMethod is null)
                return null;

            shippingPrice = deliveryMethod.Price;
        }

        foreach (var item in cart.Items)
        {
            var product = await productRepo.GetByIdAsync(item.ProductId);
            if (product is null) return null;

            item.Price = product.Price;
        }

        var service = new PaymentIntentService();
        PaymentIntent? intent = null;

        if (string.IsNullOrEmpty(cart.PaymentIntentId))
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount =(long) cart.Items.Sum(i=>i.Quantity*(i.Price *100)) + (long)(shippingPrice * 100),
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" },
            };

            intent = await service.CreateAsync(options);
        }
        else
        {
            var options = new PaymentIntentUpdateOptions
            {
                Amount = (long)cart.Items.Sum(i => i.Quantity * (i.Price * 100)) + (long)(shippingPrice * 100),
            };
            intent = await service.UpdateAsync(cart.PaymentIntentId, options);
        }

        await cartService.SetCartAsync(cart);

        return cart;
    }
}
