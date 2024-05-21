using MarketWebApp.Data;
using MarketWebApp.Models.Entity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;

namespace MarketWebApp.Controllers
{
    public class WishListController : BaseController
    {

        IHttpContextAccessor _contx;
        ApplicationDbContext context;
        List<Product> products;

        public WishListController(ApplicationDbContext _context, IHttpContextAccessor contx):base(_context)
        {
            context = _context;
            _contx = contx;
            products = new List<Product>();


        }

        [HttpPost]
        public ActionResult AddProductToWish(int id)
        {
            var wish = context.Products.Find(id);
            
            var existingWishlistData = _contx.HttpContext.Session.GetString("ProductData");
            List<Product> products = new List<Product>();

            if (!string.IsNullOrEmpty(existingWishlistData))
            {
                products = JsonConvert.DeserializeObject<List<Product>>(existingWishlistData);

                if (products.Any(p => p.ID == wish.ID ))
                {
                    TempData["DuplicateMessage"] = "This product is already in your wishlist.";
                    return RedirectToAction("Index", "Home");
                }
            }

            products.Add(wish);

            var serializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            string updatedProductString = JsonConvert.SerializeObject(products, serializerSettings);

            _contx.HttpContext.Session.SetString("ProductData", updatedProductString);
            TempData["SuccessMessage"] = "Product added to wishlist successfully.";
            return RedirectToAction("Index", "Home");

        }
        public IActionResult WishIndex()
        {

            string productString = _contx.HttpContext.Session.GetString("ProductData");
            if (!string.IsNullOrEmpty(productString))
            {

                products = JsonConvert.DeserializeObject<List<Product>>(productString);
            }

            return View(products);
        }


        public ActionResult RemoveProductFromWish(int id)
        {
            var existingWishlistData = _contx.HttpContext.Session.GetString("ProductData");
            List<Product> products = new List<Product>();

            if (!string.IsNullOrEmpty(existingWishlistData))
            {
                products = JsonConvert.DeserializeObject<List<Product>>(existingWishlistData);

                var productToRemove = products.FirstOrDefault(p => p.ID == id);
                if (productToRemove != null)
                {
                    products.Remove(productToRemove);

                    string updatedProductString = JsonConvert.SerializeObject(products);

                    _contx.HttpContext.Session.SetString("ProductData", updatedProductString);
                }
            }
            TempData["DeletedMessage"] = "Product deleted from wishlist successfully.";

            return RedirectToAction("WishIndex");
        }

    }
}
