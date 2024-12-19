using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lab7.Data;
using lab7.Models;
using System.IO;

namespace lab7.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BooksController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> CatalogueBuy()
        {
            if (!await _context.book.AnyAsync())
            {
                await SeedBooks();
            }
            ViewBag.IsAdmin = HttpContext.Session.GetString("UserRole") == "admin";
            return View(await _context.book.ToListAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserRole") != "admin")
                return RedirectToAction("Login", "UsersAccounts");
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(book book, IFormFile imageFile)
        {
            if (HttpContext.Session.GetString("UserRole") != "admin")
                return RedirectToAction("Login", "UsersAccounts");

            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    book.imgfile = uniqueFileName;
                }

                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(CatalogueBuy));
            }
            return View(book);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("UserRole") != "admin")
                return RedirectToAction("Login", "UsersAccounts");

            if (id == null)
                return NotFound();

            var book = await _context.book.FindAsync(id);
            if (book == null)
                return NotFound();

            return View(book);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, book book, IFormFile imageFile)
        {
            if (HttpContext.Session.GetString("UserRole") != "admin")
                return RedirectToAction("Login", "UsersAccounts");

            if (id != book.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(book.imgfile))
                        {
                            var oldImagePath = Path.Combine(uploadsFolder, book.imgfile);
                            if (System.IO.File.Exists(oldImagePath))
                                System.IO.File.Delete(oldImagePath);
                        }

                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        book.imgfile = uniqueFileName;
                    }

                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(CatalogueBuy));
            }
            return View(book);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.Session.GetString("UserRole") != "admin")
                return RedirectToAction("Login", "UsersAccounts");

            if (id == null)
                return NotFound();

            var book = await _context.book
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
                return NotFound();

            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "admin")
                return RedirectToAction("Login", "UsersAccounts");

            var book = await _context.book.FindAsync(id);
            if (book != null)
            {
                // Delete image file if exists
                if (!string.IsNullOrEmpty(book.imgfile))
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", book.imgfile);
                    if (System.IO.File.Exists(imagePath))
                        System.IO.File.Delete(imagePath);
                }

                _context.book.Remove(book);
                await _context.SaveChangesAsync();

                // Check if there are any books left
                if (!await _context.book.AnyAsync())
                {
                    await SeedBooks(); // Re-seed if no books exist
                }
            }
            return RedirectToAction(nameof(CatalogueBuy));
        }

        private bool BookExists(int id)
        {
            return _context.book.Any(e => e.Id == id);
        }

        public async Task<IActionResult> ViewCart()
        {
            var cartItems = HttpContext.Session.Get<List<buybook>>("Cart") ?? new List<buybook>();
            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id, int quantity)
        {
            var book = await _context.book.FindAsync(id);
            if (book == null)
                return NotFound();

            var cartItems = HttpContext.Session.Get<List<buybook>>("Cart") ?? new List<buybook>();
            
            var cartItem = new buybook
            {
                BookId = book.Id,
                Title = book.title,
                Price = book.price,
                quant = quantity
            };
            
            cartItems.Add(cartItem);
            HttpContext.Session.Set("Cart", cartItems);

            return RedirectToAction(nameof(ViewCart));
        }

        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            var userName = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(userName))
                return RedirectToAction("Login", "UsersAccounts");

            var cartItems = HttpContext.Session.Get<List<buybook>>("Cart") ?? new List<buybook>();
            if (!cartItems.Any())
                return RedirectToAction(nameof(ViewCart));

            var order = new bookorder
            {
                custname = userName,
                orderdate = DateTime.Now,
                total = cartItems.Sum(item => item.Price * item.quant)
            };

            _context.bookorder.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in cartItems)
            {
                var orderDetail = new buybook
                {
                    OrderId = order.Id,
                    BookId = item.BookId,
                    Title = item.Title,
                    Price = item.Price,
                    quant = item.quant
                };
                _context.buybook.Add(orderDetail);
            }

            await _context.SaveChangesAsync();
            HttpContext.Session.Remove("Cart");

            return RedirectToAction(nameof(MyOrders));
        }

        public async Task<IActionResult> MyOrders()
        {
            var userName = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(userName))
                return RedirectToAction("Login", "UsersAccounts");

            var orders = await _context.bookorder
                .Where(o => o.custname == userName)
                .OrderByDescending(o => o.orderdate)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> SeedBooks()
        {
            if (!await _context.book.AnyAsync())
            {
                var books = new List<book>
                {
                    new book { title = "C# Programming", price = 49.99M, imgfile = "csharp.jpg" },
                    new book { title = "ASP.NET Core MVC", price = 39.99M, imgfile = "aspnet.jpg" },
                    new book { title = "SQL Database", price = 29.99M, imgfile = "sql.jpg" }
                };

                await _context.book.AddRangeAsync(books);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(CatalogueBuy));
        }
    }
}
