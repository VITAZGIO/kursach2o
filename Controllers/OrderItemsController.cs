using FurnitureStore.Data;
using FurnitureStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStore.Controllers;

public class OrderItemsController : BaseController
{
    private readonly ApplicationDbContext _context;
    public OrderItemsController(ApplicationDbContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        if (!IsAdmin) return Forbidden();
        var items = await _context.OrderItems.Include(i => i.Order).Include(i => i.FurnitureItem).ToListAsync();
        return View(items);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        if (!IsAdmin) return Forbidden();
        var item = await _context.OrderItems.Include(i => i.Order).Include(i => i.FurnitureItem).FirstOrDefaultAsync(i => i.Id == id);
        return item == null ? NotFound() : View(item);
    }

    public IActionResult Create(int? orderId)
    {
        if (!IsAdmin) return Forbidden();
        FillSelectLists(orderId);
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OrderItem orderItem)
    {
        if (!IsAdmin) return Forbidden();
        if (ModelState.IsValid)
        {
            if (orderItem.UnitPrice == 0)
            {
                orderItem.UnitPrice = await _context.FurnitureItems
                    .Where(f => f.Id == orderItem.FurnitureItemId)
                    .Select(f => f.Price)
                    .FirstOrDefaultAsync();
            }
            _context.Add(orderItem);
            await _context.SaveChangesAsync();
            await RecalculateOrderTotal(orderItem.OrderId);
            return RedirectToAction("Details", "Orders", new { id = orderItem.OrderId });
        }
        FillSelectLists(orderItem.OrderId, orderItem.FurnitureItemId);
        return View(orderItem);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        if (!IsAdmin) return Forbidden();
        var item = await _context.OrderItems.FindAsync(id);
        if (item == null) return NotFound();
        FillSelectLists(item.OrderId, item.FurnitureItemId);
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, OrderItem orderItem)
    {
        if (!IsAdmin) return Forbidden();
        if (id != orderItem.Id) return NotFound();
        if (ModelState.IsValid)
        {
            _context.Update(orderItem);
            await _context.SaveChangesAsync();
            await RecalculateOrderTotal(orderItem.OrderId);
            return RedirectToAction("Details", "Orders", new { id = orderItem.OrderId });
        }
        FillSelectLists(orderItem.OrderId, orderItem.FurnitureItemId);
        return View(orderItem);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        if (!IsAdmin) return Forbidden();
        var item = await _context.OrderItems.Include(i => i.Order).Include(i => i.FurnitureItem).FirstOrDefaultAsync(i => i.Id == id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!IsAdmin) return Forbidden();
        var item = await _context.OrderItems.FindAsync(id);
        var orderId = item?.OrderId;
        if (item != null)
        {
            _context.OrderItems.Remove(item);
            await _context.SaveChangesAsync();
        }
        if (orderId.HasValue)
        {
            await RecalculateOrderTotal(orderId.Value);
            return RedirectToAction("Details", "Orders", new { id = orderId.Value });
        }
        return RedirectToAction(nameof(Index));
    }

    private void FillSelectLists(int? orderId = null, int? furnitureItemId = null)
    {
        ViewData["OrderId"] = new SelectList(_context.Orders.OrderByDescending(o => o.OrderDate), "Id", "Id", orderId);
        ViewData["FurnitureItemId"] = new SelectList(_context.FurnitureItems.OrderBy(f => f.Name), "Id", "Name", furnitureItemId);
    }

    private async Task RecalculateOrderTotal(int orderId)
    {
        var order = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == orderId);
        if (order == null) return;
        order.TotalAmount = order.OrderItems.Sum(i => i.Quantity * i.UnitPrice);
        await _context.SaveChangesAsync();
    }
}
