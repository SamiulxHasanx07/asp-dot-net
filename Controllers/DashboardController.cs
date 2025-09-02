using Microsoft.AspNetCore.Mvc;
using Models;
using Helpers;
using System.Linq;

namespace Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _db;

        public DashboardController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public IActionResult CancelOrder(int id)
        {
            var user = HttpContext.Session.GetString("User");
            if (user != "admin@gmail.com")
            {
                TempData["Message"] = "Access denied.";
                return RedirectToAction("Login", "Auth");
            }
            var order = _db.Orders.Find(id);
            if (order != null && order.Status == "Pending")
            {
                order.Status = "Cancelled";
                _db.SaveChanges();
                TempData["Message"] = $"Order #{id} cancelled by admin.";
            }
            return RedirectToAction("Orders");
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(int id, string status)
        {
            var user = HttpContext.Session.GetString("User");
            if (user != "admin@gmail.com")
            {
                TempData["Message"] = "Access denied.";
                return RedirectToAction("Login", "Auth");
            }
            var order = _db.Orders.Find(id);
            if (order != null)
            {
                order.Status = status;
                _db.SaveChanges();
                TempData["Message"] = $"Order #{id} status updated to {status}.";
            }
            return RedirectToAction("Orders");
        }

        public IActionResult Orders()
        {
            var user = HttpContext.Session.GetString("User");
            if (user != "admin@gmail.com")
            {
                TempData["Message"] = "Access denied.";
                return RedirectToAction("Login", "Auth");
            }
            var orders = _db.Orders.OrderByDescending(o => o.CreatedAt).ToList();
            return View(orders);
        }

        public IActionResult Index()
        {
            var products = _db.Products.ToList();
            return View(products);
        }

        [HttpPost]
        public IActionResult Add(Product product)
        {
            var file = Request.Form.Files["ImageFile"];
            if (file != null && file.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);
                var filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                product.ImageUrl = "/images/" + fileName;
            }
            else if (string.IsNullOrWhiteSpace(product.ImageUrl))
            {
                product.ImageUrl = "https://via.placeholder.com/300x220.png?text=" + product.Name.Replace(" ", "+");
            }
            _db.Products.Add(product);
            _db.SaveChanges();
            TempData["Message"] = $"Added {product.Name} successfully!";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _db.Products.Find(id);
            if (product == null)
                return RedirectToAction("Index");
            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(int id, Product updatedProduct)
        {
            var product = _db.Products.Find(id);
            if (product == null)
                return RedirectToAction("Index");

            product.Name = updatedProduct.Name;
            product.Description = updatedProduct.Description;
            product.Price = updatedProduct.Price;

            var file = Request.Form.Files["ImageFile"];
            if (file != null && file.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);
                var filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                product.ImageUrl = "/images/" + fileName;
            }
            _db.SaveChanges();
            TempData["Message"] = $"Updated {product.Name} successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var product = _db.Products.Find(id);
            if (product != null)
            {
                _db.Products.Remove(product);
                _db.SaveChanges();
                TempData["Message"] = $"Deleted {product.Name} successfully!";
            }
            return RedirectToAction("Index");
        }
    }
}
