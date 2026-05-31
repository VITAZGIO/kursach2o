using FurnitureStore.Data;
using FurnitureStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStore.Controllers;

public class CategoriesController : BaseController
{
    private readonly ApplicationDbContext _context;
    public CategoriesController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index() => View(await _context.Categories.OrderBy(c => c.Name).ToListAsync());
    public async Task<IActionResult> Details(int? id) => await FindCategoryView(id);
    public IActionResult Create() => IsAdmin ? View() : Forbidden();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category)
    {
        if (!IsAdmin) return Forbidden();
        if (!ModelState.IsValid) return View(category);
        _context.Add(category);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id) => IsAdmin ? await FindCategoryView(id) : Forbidden();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Category category)
    {
        if (!IsAdmin) return Forbidden();
        if (id != category.Id) return NotFound();
        if (!ModelState.IsValid) return View(category);
        _context.Update(category);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id) => IsAdmin ? await FindCategoryView(id) : Forbidden();

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!IsAdmin) return Forbidden();
        var category = await _context.Categories.FindAsync(id);
        if (category != null)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task<IActionResult> FindCategoryView(int? id)
    {
        if (id == null) return NotFound();
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        return category == null ? NotFound() : View(category);
    }
}
