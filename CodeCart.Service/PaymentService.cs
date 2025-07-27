using CodeCart.Core.Entities;
using CodeCart.Core.Repositories.Contracts;
using CodeCart.Core.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace CodeCart.Service;

public class PaymentService : IPaymentService
{
    private readonly IConfiguration _configuration;
    private readonly ICartService _cartService;
    private readonly IUnitOfWork _unitOfWork;

    public PaymentService(IConfiguration configuration,
        ICartService cartService,
        IUnitOfWork unitOfWork)
    {
        this._configuration = configuration;
        this._cartService = cartService;
        this._unitOfWork = unitOfWork;

        StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"] ??
            throw new InvalidOperationException("Stripe secret key is not configured.");

    }

    public async Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cartId)
    {
        
        var cart = await _cartService.GetCartAsync(cartId);
        if (cart == null) return null;

        var shippingPrice = 0m;
        if (cart.DeliveryMethodId.HasValue)
        {
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(cart.DeliveryMethodId.Value);
            if (deliveryMethod is null)
                return null;
            shippingPrice = deliveryMethod.Price;
        }

        foreach (var item in cart.Items)
        {
            var product = await _unitOfWork.Repository<Core.Entities.Product>().GetByIdAsync(item.ProductId);
            if (product is null) return null;
            item.Price = product.Price;
        }

        var service = new PaymentIntentService();
        PaymentIntent? intent = null;

        if (string.IsNullOrEmpty(cart.PaymentIntentId))
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)cart.Items.Sum(i => i.Quantity * (i.Price * 100)) + (long)(shippingPrice * 100),
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" },
            };
            intent = await service.CreateAsync(options);
            cart.PaymentIntentId = intent.Id; 
            cart.ClientSecret = intent.ClientSecret; 
        }
        else
        {
            var options = new PaymentIntentUpdateOptions
            {
                Amount = (long)cart.Items.Sum(i => i.Quantity * (i.Price * 100)) + (long)(shippingPrice * 100),
            };
            intent = await service.UpdateAsync(cart.PaymentIntentId, options);
        }

        await _cartService.SetCartAsync(cart);
        return cart;
    }

    public async Task<string> RefundPayment(string PaymentIntentId)
    {
        var refundOptions = new RefundCreateOptions()
        {
            PaymentIntent = PaymentIntentId
        };

        var refundService = new RefundService();
        var result =await refundService.CreateAsync(refundOptions);

        return result.Status;
    }
}