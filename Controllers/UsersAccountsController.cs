using Microsoft.AspNetCore.Mvc;
using lab7.Models;
using Microsoft.EntityFrameworkCore;

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
            var user = await _context.usersaccounts
                .FirstOrDefaultAsync(u => u.name == name && u.pass == pass);

            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserName", user.name);
                HttpContext.Session.SetString("UserRole", user.role);
                return RedirectToAction("CatalogueBuy", "Books");
            }

            ViewData["ErrorMessage"] = "Invalid username or password";
            return View();
        }
    }
}
