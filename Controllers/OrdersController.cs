using FurnitureStore.Data;
using FurnitureStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStore.Controllers;

public class OrdersController : BaseController
{
    private readonly ApplicationDbContext _context;
    public OrdersController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        if (!IsAdmin) return Forbidden();

        var orders = await _context.Orders
            .Include(o => o.Customer)
            .Where(o => o.Status == "В обработке")
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();

        return View(orders);
    }

    public async Task<IActionResult> My()
    {
        if (!IsLoggedIn) return RedirectToAction("Login", "Account");
        if (IsAdmin) return RedirectToAction(nameof(Index));

        var customerId = await _context.AppUsers
            .Where(u => u.Id == CurrentUserId)
            .Select(u => u.CustomerId)
            .FirstOrDefaultAsync();

        var orders = customerId == null
            ? new List<Order>()
            : await _context.Orders
                .Include(o => o.Customer)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

        return View(orders);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
            .ThenInclude(i => i.FurnitureItem)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null) return NotFound();

        if (!IsAdmin)
        {
            var customerId = await _context.AppUsers
                .Where(u => u.Id == CurrentUserId)
                .Select(u => u.CustomerId)
                .FirstOrDefaultAsync();

            if (customerId == null || order.CustomerId != customerId) return Forbidden();
        }

        return View(order);
    }

    public IActionResult Create()
    {
        if (!IsAdmin) return Forbidden();
        FillCustomers();
        return View(new Order { OrderDate = DateTime.Now, Status = "В обработке" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Order order)
    {
        if (!IsAdmin) return Forbidden();
        if (ModelState.IsValid)
        {
            order.Status = NormalizeStatus(order.Status);
            _context.Add(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        FillCustomers(order.CustomerId);
        return View(order);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        if (!IsAdmin) return Forbidden();
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound();
        FillCustomers(order.CustomerId);
        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Order order)
    {
        if (!IsAdmin) return Forbidden();
        if (id != order.Id) return NotFound();
        if (ModelState.IsValid)
        {
            order.Status = NormalizeStatus(order.Status);
            _context.Update(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        FillCustomers(order.CustomerId);
        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(int id)
    {
        if (!IsAdmin) return Forbidden();

        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound();

        order.Status = "Выполнен";
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        if (!IsAdmin) return Forbidden();
        var order = await _context.Orders.Include(o => o.Customer).FirstOrDefaultAsync(o => o.Id == id);
        return order == null ? NotFound() : View(order);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!IsAdmin) return Forbidden();
        var order = await _context.Orders.FindAsync(id);
        if (order != null)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private void FillCustomers(int? customerId = null)
    {
        ViewData["CustomerId"] = new SelectList(_context.Customers.OrderBy(c => c.FullName), "Id", "FullName", customerId);
    }

    private static string NormalizeStatus(string? status)
    {
        return status == "Выполнен" || status == "Завершён" || status == "Завершен"
            ? "Выполнен"
            : "В обработке";
    }
}
