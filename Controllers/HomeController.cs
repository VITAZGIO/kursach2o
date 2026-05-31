using System.Diagnostics;
using FurnitureStore.Data;
using FurnitureStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStore.Controllers;

public class HomeController : BaseController
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var model = new HomeViewModel
        {
            FurnitureCount = await _context.FurnitureItems.CountAsync(),
            CategoryCount = await _context.Categories.CountAsync(),
            ManufacturerCount = await _context.Manufacturers.CountAsync(),
            CustomerCount = await _context.Customers.CountAsync(),
            OrderCount = await _context.Orders.CountAsync(),
            LowStockItems = await _context.FurnitureItems
                .Include(item => item.Category)
                .Include(item => item.Manufacturer)
                .Where(item => item.StockQuantity <= 3)
                .OrderBy(item => item.StockQuantity)
                .ToListAsync()
        };

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
