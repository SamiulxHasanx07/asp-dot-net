using Microsoft.AspNetCore.Mvc;
using Models;
using Helpers;
using System.Linq;

namespace Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _db;

        public UserController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Orders()
        {
            var user = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(user))
            {
                TempData["Message"] = "Please login to view your orders.";
                return RedirectToAction("Login", "Auth");
            }
            var orders = _db.Orders.Where(o => o.CustomerEmail == user).OrderByDescending(o => o.CreatedAt).ToList();
            return View(orders);
        }

        [HttpPost]
        public IActionResult CancelOrder(int id)
        {
            var user = HttpContext.Session.GetString("User");
            var order = _db.Orders.FirstOrDefault(o => o.Id == id && o.CustomerEmail == user);
            if (order != null && order.Status == "Pending")
            {
                order.Status = "Cancelled";
                _db.SaveChanges();
                TempData["Message"] = $"Order #{id} cancelled.";
            }
            return RedirectToAction("Orders");
        }
    }
}
