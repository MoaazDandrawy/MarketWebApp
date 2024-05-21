using MarketWebApp.Data;
using MarketWebApp.Models.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace MarketWebApp.Controllers
{
    public class BaseController : Controller
    {
        ApplicationDbContext context;

        public BaseController(ApplicationDbContext _context)
        {
            context = _context;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Fetch data from the database
            ViewBag.Categories = context.Categories.ToList();
            string userId = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            ViewBag.CountCart = context.ProductCart.Include(shop => shop.ShoppingCart).Where(s => s.ShoppingCart.ApplicationUserID == userId).Count();
            var existingWishlistData = HttpContext.Session.GetString("ProductData");
            int productCount = 0;
            if (!string.IsNullOrEmpty(existingWishlistData))
            {
                JArray jsonArray = JArray.Parse(existingWishlistData);

                productCount = jsonArray.Count;
            }
            ViewBag.CountWishList = productCount;

        }
    }
}

    
