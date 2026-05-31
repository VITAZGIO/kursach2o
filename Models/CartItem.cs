using System.ComponentModel.DataAnnotations;

namespace FurnitureStore.Models;

public class CartItem
{
    public int Id { get; set; }

    [Display(Name = "Пользователь")]
    public int AppUserId { get; set; }

    [Display(Name = "Мебель")]
    public int FurnitureItemId { get; set; }

    [Range(1, 1000, ErrorMessage = "Количество должно быть больше нуля")]
    [Display(Name = "Количество")]
    public int Quantity { get; set; }

    public AppUser? AppUser { get; set; }
    public FurnitureItem? FurnitureItem { get; set; }
}
