using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStore.Models;

public class FurnitureItem
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите название мебели")]
    [Display(Name = "Название")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите артикул")]
    [Display(Name = "Артикул")]
    public string Article { get; set; } = string.Empty;

    [Display(Name = "Описание")]
    public string? Description { get; set; }

    [Display(Name = "Материал")]
    public string? Material { get; set; }

    [Display(Name = "Цвет")]
    public string? Color { get; set; }

    [Display(Name = "Размер")]
    public string? Size { get; set; }

    [Range(0, 9999999)]
    [Column(TypeName = "decimal(10,2)")]
    [Display(Name = "Цена")]
    public decimal Price { get; set; }

    [Range(0, 100000)]
    [Display(Name = "Остаток")]
    public int StockQuantity { get; set; }

    [Display(Name = "Фото")]
    public string? ImagePath { get; set; }

    [Display(Name = "Категория")]
    public int CategoryId { get; set; }

    [Display(Name = "Производитель")]
    public int ManufacturerId { get; set; }

    public Category? Category { get; set; }
    public Manufacturer? Manufacturer { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
