using FurnitureStore.Data;
using FurnitureStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStore.Controllers;

public class CustomersController : BaseController
{
    private readonly ApplicationDbContext _context;
    public CustomersController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index() => IsAdmin ? View(await _context.Customers.OrderBy(c => c.FullName).ToListAsync()) : Forbidden();
    public async Task<IActionResult> Details(int? id) => IsAdmin ? await FindCustomerView(id) : Forbidden();
    public IActionResult Create() => IsAdmin ? View() : Forbidden();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Customer customer)
    {
        if (!IsAdmin) return Forbidden();
        if (!ModelState.IsValid) return View(customer);
        _context.Add(customer);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id) => IsAdmin ? await FindCustomerView(id) : Forbidden();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Customer customer)
    {
        if (!IsAdmin) return Forbidden();
        if (id != customer.Id) return NotFound();
        if (!ModelState.IsValid) return View(customer);
        _context.Update(customer);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id) => IsAdmin ? await FindCustomerView(id) : Forbidden();

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!IsAdmin) return Forbidden();
        var customer = await _context.Customers.FindAsync(id);
        if (customer != null)
        {
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task<IActionResult> FindCustomerView(int? id)
    {
        if (id == null) return NotFound();
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
        return customer == null ? NotFound() : View(customer);
    }
}
