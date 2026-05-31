using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FurnitureStore.Models;

public class Order
{
    public int Id { get; set; }

    [Display(Name = "Клиент")]
    public int CustomerId { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Дата заказа")]
    public DateTime OrderDate { get; set; } = DateTime.Now;

    [Required(ErrorMessage = "Введите статус")]
    [Display(Name = "Статус")]
    public string Status { get; set; } = "В обработке";

    [Column(TypeName = "decimal(10,2)")]
    [Display(Name = "Сумма")]
    public decimal TotalAmount { get; set; }

    public Customer? Customer { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
