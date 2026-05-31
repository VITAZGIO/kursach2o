using FurnitureStore.Data;
using FurnitureStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStore.Controllers;

public class CartController : BaseController
{
    private readonly ApplicationDbContext _context;

    public CartController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        if (!IsLoggedIn) return RedirectToAction("Login", "Account");
        if (IsAdmin) return RedirectToAction("Index", "Orders");

        var items = await _context.CartItems
            .Include(i => i.FurnitureItem)
            .Where(i => i.AppUserId == CurrentUserId)
            .OrderBy(i => i.FurnitureItem!.Name)
            .ToListAsync();

        return View(new CartViewModel { Items = items });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int furnitureItemId)
    {
        if (!IsLoggedIn) return RedirectToAction("Login", "Account");
        if (!IsUser) return Forbidden();

        var item = await _context.CartItems.FirstOrDefaultAsync(i => i.AppUserId == CurrentUserId && i.FurnitureItemId == furnitureItemId);
        if (item == null)
        {
            _context.CartItems.Add(new CartItem { AppUserId = CurrentUserId!.Value, FurnitureItemId = furnitureItemId, Quantity = 1 });
        }
        else
        {
            item.Quantity++;
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Index", "FurnitureItems");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeQuantity(int id, int delta)
    {
        if (!IsLoggedIn) return RedirectToAction("Login", "Account");

        var item = await _context.CartItems.FirstOrDefaultAsync(i => i.Id == id && i.AppUserId == CurrentUserId);
        if (item == null) return NotFound();

        item.Quantity += delta;
        if (item.Quantity <= 0)
        {
            _context.CartItems.Remove(item);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int id)
    {
        if (!IsLoggedIn) return RedirectToAction("Login", "Account");

        var item = await _context.CartItems.FirstOrDefaultAsync(i => i.Id == id && i.AppUserId == CurrentUserId);
        if (item != null)
        {
            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout()
    {
        if (!IsLoggedIn) return RedirectToAction("Login", "Account");
        if (!IsUser) return Forbidden();

        var user = await _context.AppUsers.Include(u => u.Customer).FirstOrDefaultAsync(u => u.Id == CurrentUserId);
        if (user == null) return RedirectToAction("Login", "Account");

        var items = await _context.CartItems
            .Include(i => i.FurnitureItem)
            .Where(i => i.AppUserId == user.Id)
            .ToListAsync();

        if (!items.Any())
        {
            TempData["CartError"] = "Корзина пуста.";
            return RedirectToAction(nameof(Index));
        }

        if (items.Any(i => i.FurnitureItem == null || i.FurnitureItem.StockQuantity < i.Quantity))
        {
            TempData["CartError"] = "Недостаточно товара на складе.";
            return RedirectToAction(nameof(Index));
        }

        if (user.CustomerId == null)
        {
            var customer = new Customer
            {
                FullName = user.FullName,
                Phone = user.Phone,
                Email = user.Email,
                Address = user.Address
            };
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            user.CustomerId = customer.Id;
        }

        var order = new Order
        {
            CustomerId = user.CustomerId!.Value,
            OrderDate = DateTime.Now,
            Status = "В обработке",
            TotalAmount = items.Sum(i => i.Quantity * i.FurnitureItem!.Price)
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        foreach (var item in items)
        {
            var furniture = item.FurnitureItem!;
            _context.OrderItems.Add(new OrderItem
            {
                OrderId = order.Id,
                FurnitureItemId = furniture.Id,
                Quantity = item.Quantity,
                UnitPrice = furniture.Price
            });
            furniture.StockQuantity -= item.Quantity;
        }

        _context.CartItems.RemoveRange(items);
        await _context.SaveChangesAsync();

        TempData["CartSuccess"] = "Заказ успешно оформлен.";
        return RedirectToAction(nameof(Index));
    }
}
