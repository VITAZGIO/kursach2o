using System.ComponentModel.DataAnnotations;

namespace FurnitureStore.Models;

public class AppUser
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите логин")]
    [Display(Name = "Логин")]
    public string Login { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите пароль")]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Роль")]
    public string Role { get; set; } = "User";

    [Required(ErrorMessage = "Введите ФИО")]
    [Display(Name = "ФИО")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Телефон")]
    public string? Phone { get; set; }

    [EmailAddress(ErrorMessage = "Введите корректный email")]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Display(Name = "Адрес")]
    public string? Address { get; set; }

    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
