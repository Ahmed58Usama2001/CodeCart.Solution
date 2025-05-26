namespace CodeCart.Core.Entities;

public class ShoppingCart
{
    public string Id { get; set; }

    public List<CartItem> Items { get; set; } = new();
}
