namespace FurnitureStore.Models;

public class HomeViewModel
{
    public int FurnitureCount { get; set; }
    public int CategoryCount { get; set; }
    public int ManufacturerCount { get; set; }
    public int CustomerCount { get; set; }
    public int OrderCount { get; set; }
    public List<FurnitureItem> LowStockItems { get; set; } = new();
}
