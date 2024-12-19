using Microsoft.AspNetCore.Mvc;
using lab7.Models;
using Microsoft.EntityFrameworkCore;
using lab7.Data;

namespace lab7.Controllers
{
    public class UsersAccountsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersAccountsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string name, string pass)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(pass))
            {
                ViewData["ErrorMessage"] = "Please enter both username and password";
                return View();
            }

            var user = await _context.usersaccounts
                .FirstOrDefaultAsync(u => u.name == name && u.pass == pass);

            if (user != null)
            {
                HttpContext.Session.SetString("UserName", user.name);
                HttpContext.Session.SetString("UserRole", user.role);
                return RedirectToAction("CatalogueBuy", "Books");
            }

            ViewData["ErrorMessage"] = "Invalid username or password";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> SeedUsers()
        {
            if (!await _context.usersaccounts.AnyAsync())
            {
                var users = new List<usersaccounts>
                {
                    new usersaccounts { name = "admin", pass = "admin123", role = "admin" },
                    new usersaccounts { name = "john", pass = "john123", role = "customer" },
                    new usersaccounts { name = "jane", pass = "jane123", role = "customer" }
                };

                await _context.usersaccounts.AddRangeAsync(users);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Login");
        }
    }
}
