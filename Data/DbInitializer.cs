using FurnitureStore.Models;

namespace FurnitureStore.Data;

public static class DbInitializer
{
    public static void Seed(ApplicationDbContext context)
    {
        if (!context.AppUsers.Any(user => user.Login == "admin"))
        {
            context.AppUsers.Add(new AppUser
            {
                Login = "admin",
                Password = "admin",
                Role = "Admin",
                FullName = "Администратор"
            });
            context.SaveChanges();
        }

        NormalizeOrderStatuses(context);
        UpdateFurnitureImagePaths(context);

        if (context.Categories.Any())
        {
            return;
        }

        var categories = new List<Category>
        {
            new() { Name = "Диваны", Description = "Мягкая мебель для гостиной" },
            new() { Name = "Столы", Description = "Обеденные и письменные столы" },
            new() { Name = "Шкафы", Description = "Шкафы для одежды и хранения" },
            new() { Name = "Кровати", Description = "Мебель для спальни" }
        };

        var manufacturers = new List<Manufacturer>
        {
            new() { Name = "ДомМебель", Country = "Россия", Phone = "+7 (495) 111-22-33", Email = "info@dommebel.ru" },
            new() { Name = "Nord Wood", Country = "Беларусь", Phone = "+375 (17) 222-44-55", Email = "sales@nordwood.by" },
            new() { Name = "Comfort Line", Country = "Россия", Phone = "+7 (812) 555-90-10", Email = "order@comfortline.ru" }
        };

        var customers = new List<Customer>
        {
            new() { FullName = "Иванов Сергей Петрович", Phone = "+7 (900) 123-45-67", Email = "ivanov@mail.ru", Address = "г. Москва, ул. Лесная, д. 4" },
            new() { FullName = "Петрова Анна Викторовна", Phone = "+7 (901) 222-33-44", Email = "petrova@mail.ru", Address = "г. Тула, пр. Мира, д. 18" },
            new() { FullName = "Смирнов Олег Андреевич", Phone = "+7 (902) 777-88-99", Email = "smirnov@mail.ru", Address = "г. Калуга, ул. Центральная, д. 9" }
        };

        context.Categories.AddRange(categories);
        context.Manufacturers.AddRange(manufacturers);
        context.Customers.AddRange(customers);
        context.SaveChanges();

        var furniture = new List<FurnitureItem>
        {
            new() { Name = "Диван Прага", Article = "DIV-1001", Description = "Удобный прямой диван для гостиной", Material = "Велюр", Color = "Бежевый", Size = "220x95x85 см", Price = 45900, StockQuantity = 4, ImagePath = "/images/Sofa.jpg", CategoryId = categories[0].Id, ManufacturerId = manufacturers[0].Id },
            new() { Name = "Стол Милан", Article = "STL-2102", Description = "Обеденный стол из массива", Material = "Дуб", Color = "Орех", Size = "160x90x75 см", Price = 28900, StockQuantity = 8, ImagePath = "/images/Table.jpg", CategoryId = categories[1].Id, ManufacturerId = manufacturers[1].Id },
            new() { Name = "Шкаф Лайт", Article = "SHK-3303", Description = "Двухстворчатый шкаф для спальни", Material = "ЛДСП", Color = "Светлый дуб", Size = "120x55x210 см", Price = 33500, StockQuantity = 2, ImagePath = "/images/Wardrobe.jpg", CategoryId = categories[2].Id, ManufacturerId = manufacturers[2].Id },
            new() { Name = "Кровать Соната", Article = "KRV-4404", Description = "Кровать с мягким изголовьем", Material = "Ткань, дерево", Color = "Кофейный", Size = "180x200 см", Price = 52800, StockQuantity = 5, ImagePath = "/images/Bed.jpg", CategoryId = categories[3].Id, ManufacturerId = manufacturers[0].Id },
            new() { Name = "Журнальный столик Вега", Article = "STL-2505", Description = "Компактный столик для гостиной", Material = "МДФ", Color = "Белый", Size = "90x55x45 см", Price = 9900, StockQuantity = 1, ImagePath = "/images/coffee-table.jpg", CategoryId = categories[1].Id, ManufacturerId = manufacturers[2].Id }
        };

        context.FurnitureItems.AddRange(furniture);
        context.SaveChanges();
        UpdateFurnitureImagePaths(context);

        var orders = new List<Order>
        {
            new() { CustomerId = customers[0].Id, OrderDate = DateTime.Today.AddDays(-7).AddHours(12).AddMinutes(20), Status = "Выполнен", TotalAmount = 45900 },
            new() { CustomerId = customers[1].Id, OrderDate = DateTime.Today.AddDays(-3).AddHours(15).AddMinutes(10), Status = "В обработке", TotalAmount = 62400 },
            new() { CustomerId = customers[2].Id, OrderDate = DateTime.Today.AddDays(-1).AddHours(10).AddMinutes(45), Status = "В обработке", TotalAmount = 9900 }
        };

        context.Orders.AddRange(orders);
        context.SaveChanges();

        context.OrderItems.AddRange(
            new OrderItem { OrderId = orders[0].Id, FurnitureItemId = furniture[0].Id, Quantity = 1, UnitPrice = furniture[0].Price },
            new OrderItem { OrderId = orders[1].Id, FurnitureItemId = furniture[1].Id, Quantity = 1, UnitPrice = furniture[1].Price },
            new OrderItem { OrderId = orders[1].Id, FurnitureItemId = furniture[2].Id, Quantity = 1, UnitPrice = furniture[2].Price },
            new OrderItem { OrderId = orders[2].Id, FurnitureItemId = furniture[4].Id, Quantity = 1, UnitPrice = furniture[4].Price });

        context.SaveChanges();
    }

    private static void NormalizeOrderStatuses(ApplicationDbContext context)
    {
        if (!context.Orders.Any()) return;

        var changed = false;
        foreach (var order in context.Orders)
        {
            var normalizedStatus = order.Status == "Выполнен"
                || order.Status == "Завершён"
                || order.Status == "Завершен"
                || order.Status.Contains("Выполн", StringComparison.OrdinalIgnoreCase)
                || order.Status.Contains("Заверш", StringComparison.OrdinalIgnoreCase)
                || order.Status.Contains("Р’С‹Рї", StringComparison.OrdinalIgnoreCase)
                || order.Status.Contains("Р—Р°РІ", StringComparison.OrdinalIgnoreCase)
                ? "Выполнен"
                : "В обработке";

            if (order.Status != normalizedStatus)
            {
                order.Status = normalizedStatus;
                changed = true;
            }
        }

        if (changed)
        {
            context.SaveChanges();
        }
    }

    private static void UpdateFurnitureImagePaths(ApplicationDbContext context)
    {
        if (!context.FurnitureItems.Any()) return;

        var imagePathsByArticle = new Dictionary<string, string>
        {
            ["DIV-1001"] = "/images/Sofa.jpg",
            ["STL-2505"] = "/images/coffee-table.jpg",
            ["KRV-4404"] = "/images/Bed.jpg",
            ["STL-2102"] = "/images/Table.jpg",
            ["SHK-3303"] = "/images/Wardrobe.jpg"
        };

        var changed = false;
        foreach (var item in context.FurnitureItems)
        {
            if (imagePathsByArticle.TryGetValue(item.Article, out var imagePath) && item.ImagePath != imagePath)
            {
                item.ImagePath = imagePath;
                changed = true;
            }
        }

        if (changed)
        {
            context.SaveChanges();
        }
    }
}
