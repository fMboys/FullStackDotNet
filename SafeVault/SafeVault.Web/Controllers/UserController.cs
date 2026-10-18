using Microsoft.AspNetCore.Mvc;
using SafeVault.Web.Data;
using SafeVault.Web.Helpers;
using SafeVault.Web.Models;

namespace SafeVault.Web.Controllers;

public class UserController : Controller
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(
        string username,
        string email,
        string password)
    {
        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            return BadRequest();
        }

        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash =
                PasswordService.HashPassword(password)
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return RedirectToAction("Login", "Account");
    }
}