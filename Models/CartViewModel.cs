namespace FurnitureStore.Models;

public class CartViewModel
{
    public List<CartItem> Items { get; set; } = new();
    public decimal TotalAmount => Items.Sum(item => item.Quantity * (item.FurnitureItem?.Price ?? 0));
}
