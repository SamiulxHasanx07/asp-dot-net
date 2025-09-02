using Microsoft.AspNetCore.Mvc;
using Models;
using System.Collections.Generic;
using System.Linq;

namespace Controllers
{
    public class AuthController : Controller
    {
        private readonly Helpers.AppDbContext _db;

        public AuthController(Helpers.AppDbContext db)
        {
            _db = db;
        }

        // Seed admin user if not exists
        private void EnsureAdminUser()
        {
            var admin = _db.Users.FirstOrDefault(u => u.Username == "admin@gmail.com");
            if (admin == null)
            {
                _db.Users.Add(new User { Username = "admin@gmail.com", Password = "admin123" });
                _db.SaveChanges();
            }
        }

        public IActionResult Login()
        {
            EnsureAdminUser();
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                HttpContext.Session.SetString("User", username);
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Error = "Invalid username or password.";
            return View();
        }

        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Signup(string username, string password)
        {
            if (_db.Users.Any(u => u.Username == username))
            {
                ViewBag.Error = "Username already exists.";
                return View();
            }
            var user = new User { Username = username, Password = password };
            _db.Users.Add(user);
            _db.SaveChanges();
            HttpContext.Session.SetString("User", username);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("User");
            return RedirectToAction("Index", "Home");
        }
    }
}
