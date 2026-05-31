using FurnitureStore.Data;
using FurnitureStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStore.Controllers;

public class ManufacturersController : BaseController
{
    private readonly ApplicationDbContext _context;
    public ManufacturersController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index() => View(await _context.Manufacturers.OrderBy(m => m.Name).ToListAsync());
    public async Task<IActionResult> Details(int? id) => await FindManufacturerView(id);
    public IActionResult Create() => IsAdmin ? View() : Forbidden();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Manufacturer manufacturer)
    {
        if (!IsAdmin) return Forbidden();
        if (!ModelState.IsValid) return View(manufacturer);
        _context.Add(manufacturer);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id) => IsAdmin ? await FindManufacturerView(id) : Forbidden();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Manufacturer manufacturer)
    {
        if (!IsAdmin) return Forbidden();
        if (id != manufacturer.Id) return NotFound();
        if (!ModelState.IsValid) return View(manufacturer);
        _context.Update(manufacturer);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id) => IsAdmin ? await FindManufacturerView(id) : Forbidden();

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!IsAdmin) return Forbidden();
        var manufacturer = await _context.Manufacturers.FindAsync(id);
        if (manufacturer != null)
        {
            _context.Manufacturers.Remove(manufacturer);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task<IActionResult> FindManufacturerView(int? id)
    {
        if (id == null) return NotFound();
        var manufacturer = await _context.Manufacturers.FirstOrDefaultAsync(m => m.Id == id);
        return manufacturer == null ? NotFound() : View(manufacturer);
    }
}
