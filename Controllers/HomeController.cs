using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Models;
using Helpers;

namespace Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Helpers.AppDbContext _db;

        public HomeController(ILogger<HomeController> logger, Helpers.AppDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpPost]
        public IActionResult Checkout(string CustomerName, string CustomerEmail, string CustomerAddress)
        {
            var user = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(user))
            {
                TempData["Message"] = "Please login to place an order.";
                return RedirectToAction("Login", "Auth");
            }
            var cart = GetCart();
            if (cart.Items.Count == 0)
            {
                TempData["Message"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }
            var paymentMethod = Request.Form["PaymentMethod"];
            var transactionId = Request.Form["TransactionId"];
            var order = new Order
            {
                CustomerName = CustomerName,
                CustomerEmail = CustomerEmail,
                CustomerAddress = CustomerAddress,
                PaymentMethod = paymentMethod,
                TransactionId = (paymentMethod == "Bkash" && !string.IsNullOrEmpty(transactionId.ToString())) ? transactionId.ToString() : string.Empty,
                Status = paymentMethod == "Bkash" ? "Pending" : "CashOnDelivery",
                CreatedAt = DateTime.Now,
                Items = cart.Items.Select(i => new OrderItem
                {
                    ProductId = i.Product.Id,
                    ProductName = i.Product.Name,
                    Price = i.Product.Price,
                    Quantity = i.Quantity
                }).ToList()
            };
            _db.Orders.Add(order);
            _db.SaveChanges();
            SaveCart(new Cart());
            TempData["Message"] = $"Thank you, {CustomerName}! Your order has been placed.";
            return RedirectToAction("Orders");
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            var user = HttpContext.Session.GetString("User");
            if (string.IsNullOrEmpty(user))
            {
                TempData["Message"] = "Please login to checkout.";
                return RedirectToAction("Login", "Auth");
            }
            var cart = GetCart();
            if (cart.Items.Count == 0)
            {
                TempData["Message"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }
            return View(cart);
        }

        public IActionResult Orders()
        {
            // For demo, show all orders. In real app, filter by user.
            var orders = _db.Orders.OrderByDescending(o => o.CreatedAt).ToList();
            return View("~/Views/Orders/Index.cshtml", orders);
        }

        public IActionResult Index()
        {
            var products = _db.Products.ToList();
            return View(products);
        }

        private List<Product> GetProducts()
        {
            return _db.Products.ToList();
        }

        private Cart GetCart()
        {
            var cart = HttpContext.Session.GetObjectFromJson<Cart>("Cart");
            if (cart == null)
            {
                cart = new Cart();
            }
            return cart;
        }

        private void SaveCart(Cart cart)
        {
            HttpContext.Session.SetObjectAsJson("Cart", cart);
        }

        [HttpPost]
        public IActionResult AddToCart(int id)
        {
            var products = GetProducts();
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product == null || product.Name == null)
            {
                TempData["Message"] = "Product not found.";
                return RedirectToAction("Index");
            }
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(i => i.Product != null && i.Product.Id == id);
            if (item != null)
            {
                item.Quantity++;
            }
            else
            {
                cart.Items.Add(new CartItem { Product = product, Quantity = 1 });
            }
            SaveCart(cart);
            TempData["Message"] = $"Added {product.Name} to cart!";
            return RedirectToAction("Index");
        }

        public IActionResult ViewCart()
        {
            var cart = GetCart();
            // Ensure all cart items have non-null Product
            cart.Items = cart.Items.Where(i => i.Product != null).ToList();
            return View(cart);
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int id)
        {
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(i => i.Product != null && i.Product.Id == id);
            if (item != null)
            {
                cart.Items.Remove(item);
                SaveCart(cart);
                TempData["Message"] = "Item removed from cart.";
            }
            return RedirectToAction("ViewCart");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
