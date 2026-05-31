using FurnitureStore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStore.Controllers;

public class OrderHistoryController : BaseController
{
    private readonly ApplicationDbContext _context;
    public OrderHistoryController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        if (!IsAdmin) return Forbidden();

        var orders = await _context.Orders
            .Include(o => o.Customer)
            .OrderByDescending(o => o.OrderDate)
            .ThenByDescending(o => o.Id)
            .ToListAsync();

        return View(orders);
    }
}
