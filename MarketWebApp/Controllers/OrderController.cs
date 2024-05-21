using MarketWebApp.Data;
using MarketWebApp.Models.Entity;
using MarketWebApp.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketWebApp.Controllers
{
    public class OrderController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PlaceOrder()
        {
            string userId = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // Get the shopping cart for the current user
            var shoppingCart = _context.ShoppingCart
                                        .Include(sc => sc.ProductCarts)
                                            .ThenInclude(pc => pc.Product)
                                        .FirstOrDefault(sc => sc.ApplicationUserID == userId);

            if (shoppingCart == null || shoppingCart.ProductCarts.Count == 0)
            {
                ViewBag.Message = "Your shopping cart is empty.";
                return View(new List<ProductCart>());
            }

            // Retrieve available locations from the database
            var locations = _context.Locations.ToList();


            // Create a view model to pass data to the view
            var viewModel = new PlaceOrderViewModel
            {
                ShoppingCart = shoppingCart,
                Locations = locations
            };

            return View(viewModel);

        }


        //confirm
        [HttpPost]
        [HttpGet]
        public IActionResult ConfirmOrder(PlaceOrderViewModel viewModel)
        {
            int selectedLocationId = viewModel.SelectedLocationId;

            string userId = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var shoppingCart = _context.ShoppingCart
                                        .Include(sc => sc.ProductCarts)
                                            .ThenInclude(pc => pc.Product)
                                        .FirstOrDefault(sc => sc.ApplicationUserID == userId);

            Order order = new Order
            {
                Date = DateTime.Now,
                State = "Pending", 
                LocationId = selectedLocationId,
                ApplicationUserID = userId
            };

            _context.Orders.Add(order);

            _context.SaveChanges();

            foreach (var productCart in shoppingCart.ProductCarts)
            {
                OrderProduct orderProduct = new OrderProduct
                {
                    OrderId = order.ID, 
                    ProductId = productCart.ProductId,
                    Quantity = productCart.Quantity,
                    Price = productCart.Product.Price 
                };

                _context.OrderProduct.Add(orderProduct);
            }

            _context.SaveChanges();

            _context.ShoppingCart.Remove(shoppingCart);
            _context.SaveChanges();

            // return RedirectToAction("OrderConfirmation", new { orderId = order.ID });
            return RedirectToAction("OrderHistory", "order");
        }



        public IActionResult OrderHistory()
        {
            // Get the current user's ID
            string userId = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // Retrieve order products for the current user from the database
            var order = _context.Orders
                .Where(op => op.ApplicationUserID == userId)
                .ToList();

            // Pass order products to the view
            return View(order);
        }
        public IActionResult OrderInfo(int id)
        {
            var orderproduct = _context.OrderProduct.Include(p => p.Product).Where(o => o.OrderId == id).ToList();
            return View(orderproduct);
        }
        public IActionResult CancelOrder(int id)
        {
            string userId = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var order = _context.Orders.Where(o => o.ApplicationUserID == userId).FirstOrDefault(o => o.ID == id);
            _context.Orders.Remove(order);
            _context.SaveChanges();
            return RedirectToAction("OrderHistory");
        }
    }
}
