using Microsoft.AspNetCore.Mvc;

namespace FurnitureStore.Controllers;

public abstract class BaseController : Controller
{
    protected int? CurrentUserId => HttpContext.Session.GetInt32("UserId");
    protected string? CurrentUserName => HttpContext.Session.GetString("FullName");
    protected string? CurrentUserRole => HttpContext.Session.GetString("Role");
    protected bool IsLoggedIn => CurrentUserId.HasValue;
    protected bool IsAdmin => CurrentUserRole == "Admin";
    protected bool IsUser => CurrentUserRole == "User";

    protected IActionResult Forbidden()
    {
        return View("~/Views/Shared/Forbidden.cshtml");
    }

    protected IActionResult RequireAdmin()
    {
        return IsAdmin ? new EmptyResult() : Forbidden();
    }

    protected IActionResult RequireLogin()
    {
        return IsLoggedIn ? new EmptyResult() : RedirectToAction("Login", "Account");
    }
}
