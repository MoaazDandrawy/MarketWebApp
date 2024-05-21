using MarketWebApp.Data;
using MarketWebApp.Models;
using MarketWebApp.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketWebApp.Controllers
{
    public class ShoppingCartController : BaseController
    {

        private readonly ApplicationDbContext _context;

        public ShoppingCartController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IActionResult Delete(int? id)
        {
            var sh = _context.ProductCart.FirstOrDefault(s=>s.ID==id);
            if (sh == null)
            {
                return NotFound();
            }

            _context.ProductCart.Remove(sh);
            _context.SaveChanges();
            TempData["DeletedMessage"] = "Product deleted from ShoppingCart successfully.";
            return RedirectToAction("Index");
        }




        [HttpPost]
        [Authorize]
        public IActionResult AddToCart(int id)
        {
            string userId = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;


            if (userId == null)
            {
                TempData["UserNotFound"] = "Please log in to add items to your cart.";
                return RedirectToAction("Index", "Home");
            }

          
            var shoppingCart = _context.ShoppingCart.Include(sc => sc.ProductCarts)
                                        .FirstOrDefault(cart => cart.ApplicationUserID == userId);


            if (shoppingCart == null)
            {
                shoppingCart = new ShoppingCart
                {
                    ApplicationUserID = userId,
                    ProductCarts = new List<ProductCart>()
                };
                _context.ShoppingCart.Add(shoppingCart);
            }

            var existingProductCart = shoppingCart.ProductCarts.FirstOrDefault(pc => pc.ProductId == id);

            if (existingProductCart != null)
            {
                //existingProductCart.Quantity++;
                TempData["DuplicateMessage"] = "This product is already in your shopping cart.";
            }
            else
            {
                var newProductCart = new ProductCart
                {
                    ProductId = id,
                    Quantity = 1
                };
                shoppingCart.ProductCarts.Add(newProductCart);
            TempData["SuccessMessage"] = "Product added to ShoppingCart successfully.";
            }

            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult UpdateQuantity(int id, int quantity)
        {
            string userId = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                TempData["UserNotFound"] = "Please log in to add items to your cart.";
                return RedirectToAction("Index", "Home");
            }

            var shoppingCart = _context.ShoppingCart
                .Include(sc => sc.ProductCarts)
                .FirstOrDefault(cart => cart.ApplicationUserID == userId);

            if (shoppingCart == null)
            {
                shoppingCart = new ShoppingCart
                {
                    ApplicationUserID = userId,
                    ProductCarts = new List<ProductCart>()
                };
                _context.ShoppingCart.Add(shoppingCart);
            }

            var existingProductCart = shoppingCart.ProductCarts.FirstOrDefault(pc => pc.ProductId == id);

            if (existingProductCart != null)
            {
                existingProductCart.Quantity = quantity;
            }
            else
            {
                var newProductCart = new ProductCart
                {
                    ProductId = id,
                    Quantity = quantity
                };
                shoppingCart.ProductCarts.Add(newProductCart);
            }

            _context.SaveChanges();
            return RedirectToAction("Index", "shoppingcart");
        }
    


    public IActionResult Index()
        {
            string userId = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            // Retrieve the shopping cart associated with the user
            var shoppingCart = _context.ShoppingCart
                                        .Include(cart => cart.ProductCarts)
                                            .ThenInclude(pc => pc.Product)
                                        .FirstOrDefault(cart => cart.ApplicationUserID == userId);

            if (shoppingCart == null)
            {
                // Handle the case where the user's shopping cart is empty
                TempData["EmptyShoppingCart"]= "Your shopping cart is empty.";
                return View(new List<ProductCart>());
            }

            // Pass the products in the shopping cart to the view
            return View(shoppingCart.ProductCarts);
        }


    }
}
