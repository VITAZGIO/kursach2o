using System.ComponentModel.DataAnnotations;

namespace FurnitureStore.Models;

public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите название категории")]
    [Display(Name = "Название")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Описание")]
    public string? Description { get; set; }

    public ICollection<FurnitureItem> FurnitureItems { get; set; } = new List<FurnitureItem>();
}
