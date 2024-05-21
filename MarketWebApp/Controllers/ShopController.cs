using MarketWebApp.Data;
using MarketWebApp.Models.Entity;
using Microsoft.AspNetCore.Mvc;

namespace MarketWebApp.Controllers
{
    public class ShopController : BaseController
    {
        ApplicationDbContext context;
        public ShopController(ApplicationDbContext _context) : base(_context)
        {
            context = _context;
        }
        public IActionResult Index(int page = 1)
        {
            int pageSize = 9; // Number of items per page

            var totalItems = context.Products.Where(Product => Product.Discount == 0 && Product.Stock > 0).Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var products = context.Products
                                        .Where(Product => Product.Discount == 0 && Product.Stock > 0)
                                        .Skip((page - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToList();
            var Model = new
            {
                Products = products,
                CurrentPage = page,
                Count = products.Count(),
                TotalPages = totalPages,
                Discount = context.Products.Where(Product => Product.Discount > 0 && Product.Stock > 0).ToList(),
            };
            return View(Model);
        }

        [HttpGet]
        public IActionResult Product(int id)
        {
            var products = context.Products.Where(P => P.CategoryId == id && P.Discount == 0 && P.Stock > 0);
            var Model = new
            {
                Products = products,
                Count = products.Count(),
                Discount = context.Products.Where(Product => Product.Discount > 0 && Product.Stock > 0).ToList(),
            };
            return View(Model);

        }
    }
}
