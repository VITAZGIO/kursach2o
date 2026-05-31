using System.ComponentModel.DataAnnotations;

namespace FurnitureStore.Models;

public class Customer
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите ФИО клиента")]
    [Display(Name = "ФИО")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Телефон")]
    public string? Phone { get; set; }

    [EmailAddress(ErrorMessage = "Введите корректный email")]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Display(Name = "Адрес")]
    public string? Address { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
