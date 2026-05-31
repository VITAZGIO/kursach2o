using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStore.Models;

public class OrderItem
{
    public int Id { get; set; }

    [Display(Name = "Заказ")]
    public int OrderId { get; set; }

    [Display(Name = "Мебель")]
    public int FurnitureItemId { get; set; }

    [Range(1, 1000)]
    [Display(Name = "Количество")]
    public int Quantity { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    [Display(Name = "Цена за единицу")]
    public decimal UnitPrice { get; set; }

    public Order? Order { get; set; }
    public FurnitureItem? FurnitureItem { get; set; }
}
