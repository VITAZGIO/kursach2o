using System.ComponentModel.DataAnnotations;

namespace FurnitureStore.Models;

public class Manufacturer
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите название производителя")]
    [Display(Name = "Название")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Страна")]
    public string? Country { get; set; }

    [Display(Name = "Телефон")]
    public string? Phone { get; set; }

    [EmailAddress(ErrorMessage = "Введите корректный email")]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    public ICollection<FurnitureItem> FurnitureItems { get; set; } = new List<FurnitureItem>();
}
