using FurnitureStore.Data;
using FurnitureStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStore.Controllers;

public class FurnitureItemsController : BaseController
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public FurnitureItemsController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> Index(string? search)
    {
        var items = _context.FurnitureItems
            .Include(f => f.Category)
            .Include(f => f.Manufacturer)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            items = items.Where(f =>
                f.Name.Contains(search) ||
                f.Article.Contains(search) ||
                (f.Manufacturer != null && f.Manufacturer.Name.Contains(search)));
        }

        ViewBag.Search = search;
        return View(await items.OrderBy(f => f.Name).ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var item = await _context.FurnitureItems.Include(f => f.Category).Include(f => f.Manufacturer).FirstOrDefaultAsync(f => f.Id == id);
        return item == null ? NotFound() : View(item);
    }

    public IActionResult Create()
    {
        if (!IsAdmin) return Forbidden();
        FillSelectLists();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FurnitureItem furnitureItem, IFormFile? photo)
    {
        if (!IsAdmin) return Forbidden();
        var uploadedPath = await SavePhoto(photo);
        if (uploadedPath == "invalid")
        {
            ModelState.AddModelError("ImagePath", "Можно загружать только изображения JPG, PNG или WEBP.");
        }

        if (ModelState.IsValid)
        {
            if (!string.IsNullOrEmpty(uploadedPath))
            {
                furnitureItem.ImagePath = uploadedPath;
            }
            _context.Add(furnitureItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        FillSelectLists(furnitureItem.CategoryId, furnitureItem.ManufacturerId);
        return View(furnitureItem);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        if (!IsAdmin) return Forbidden();
        var item = await _context.FurnitureItems.FindAsync(id);
        if (item == null) return NotFound();
        FillSelectLists(item.CategoryId, item.ManufacturerId);
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, FurnitureItem furnitureItem, IFormFile? photo)
    {
        if (!IsAdmin) return Forbidden();
        if (id != furnitureItem.Id) return NotFound();
        var oldItem = await _context.FurnitureItems.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
        if (oldItem == null) return NotFound();

        var uploadedPath = await SavePhoto(photo);
        if (uploadedPath == "invalid")
        {
            ModelState.AddModelError("ImagePath", "Можно загружать только изображения JPG, PNG или WEBP.");
        }

        if (ModelState.IsValid)
        {
            furnitureItem.ImagePath = string.IsNullOrEmpty(uploadedPath) ? oldItem.ImagePath : uploadedPath;
            _context.Update(furnitureItem);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        FillSelectLists(furnitureItem.CategoryId, furnitureItem.ManufacturerId);
        return View(furnitureItem);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        if (!IsAdmin) return Forbidden();
        var item = await _context.FurnitureItems.Include(f => f.Category).Include(f => f.Manufacturer).FirstOrDefaultAsync(f => f.Id == id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!IsAdmin) return Forbidden();
        var item = await _context.FurnitureItems.FindAsync(id);
        if (item != null)
        {
            _context.FurnitureItems.Remove(item);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private void FillSelectLists(int? categoryId = null, int? manufacturerId = null)
    {
        ViewData["CategoryId"] = new SelectList(_context.Categories.OrderBy(c => c.Name), "Id", "Name", categoryId);
        ViewData["ManufacturerId"] = new SelectList(_context.Manufacturers.OrderBy(m => m.Name), "Id", "Name", manufacturerId);
    }

    private async Task<string?> SavePhoto(IFormFile? photo)
    {
        if (photo == null || photo.Length == 0) return null;

        var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(photo.FileName).ToLowerInvariant();
        if (!allowed.Contains(extension)) return "invalid";

        var uploadFolder = Path.Combine(_environment.WebRootPath, "uploads", "furniture");
        Directory.CreateDirectory(uploadFolder);
        var fileName = $"{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(uploadFolder, fileName);

        using var stream = new FileStream(fullPath, FileMode.Create);
        await photo.CopyToAsync(stream);
        return $"/uploads/furniture/{fileName}";
    }
}
