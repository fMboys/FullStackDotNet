using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SafeVault.Web.Data;
using SafeVault.Web.Helpers;
using SafeVault.Web.Models;
using System.Security.Claims;

namespace UserRegistration.Web.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _context;

    public AccountController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = _context.Users
            .SingleOrDefault(u =>
                u.Username == model.Username);

        if (user == null)
        {
            ModelState.AddModelError(
                "",
                "Invalid username or password.");

            return View(model);
        }

        bool isValid =
            PasswordService.VerifyPassword(
                model.Password,
                user.PasswordHash);

        if (!isValid)
        {
            ModelState.AddModelError(
                "",
                "Invalid username or password.");

            return View(model);
        }

        var claims = new List<Claim>
        {
            new Claim(
                ClaimTypes.Name,
                user.Username),

            new Claim(
                ClaimTypes.Role,
                user.Role)
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal =
            new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal);

        return RedirectToAction(
            "Dashboard",
            "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();

        return RedirectToAction(nameof(Login));
    }
}