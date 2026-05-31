using FurnitureStore.Data;
using FurnitureStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FurnitureStore.Controllers;

public class AccountController : BaseController
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.Login == model.Login && u.Password == model.Password);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Неверный логин или пароль.");
            return View(model);
        }

        SignIn(user);
        return user.Role == "Admin"
            ? RedirectToAction("Index", "Home")
            : RedirectToAction("Index", "FurnitureItems");
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        if (await _context.AppUsers.AnyAsync(u => u.Login == model.Login))
        {
            ModelState.AddModelError(nameof(model.Login), "Пользователь с таким логином уже существует.");
            return View(model);
        }

        var customer = new Customer
        {
            FullName = model.FullName,
            Phone = model.Phone,
            Email = model.Email,
            Address = model.Address
        };

        var user = new AppUser
        {
            Login = model.Login,
            Password = model.Password,
            Role = "User",
            FullName = model.FullName,
            Phone = model.Phone,
            Email = model.Email,
            Address = model.Address,
            Customer = customer
        };

        _context.AppUsers.Add(user);
        await _context.SaveChangesAsync();
        SignIn(user);
        return RedirectToAction("Index", "FurnitureItems");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    private void SignIn(AppUser user)
    {
        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("Login", user.Login);
        HttpContext.Session.SetString("FullName", user.FullName);
        HttpContext.Session.SetString("Role", user.Role);
    }
}
