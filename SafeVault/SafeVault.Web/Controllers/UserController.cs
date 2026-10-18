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

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(User user)
    {
        if (!ModelState.IsValid)
            return View(user);

        // Detect malicious input
        if (InputSanitizer.ContainsMaliciousContent(user.Username) ||
            InputSanitizer.ContainsMaliciousContent(user.Email))
        {
            ModelState.AddModelError("", "Potentially harmful input detected.");
            return View(user);
        }

        // Sanitize values
        user.Username = InputSanitizer.Sanitize(user.Username);
        user.Email = InputSanitizer.Sanitize(user.Email);

        _context.Users.Add(user);
        _context.SaveChanges();

        return RedirectToAction("Register");
    }
}