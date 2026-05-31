using System.ComponentModel.DataAnnotations;

namespace FurnitureStore.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Введите логин")]
    [Display(Name = "Логин")]
    public string Login { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите пароль")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = string.Empty;

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
}
