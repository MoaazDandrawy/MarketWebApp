using MarketWebApp.Data;
using MarketWebApp.Repository.ProductRepository;
using MarketWebApp.Repository.SupplierRepository;
using MarketWebApp.Reprository.CategoryReprositry;
using MarketWebApp.Reprository.OrderReprository;
using Microsoft.AspNetCore.Mvc;
namespace MarketWebApp.Controllers
{
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IProductRepository productRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly ISupplierRepository supplierRepository;
        private readonly IOrderAdminRepository orderRepository;

        public ReportController(ApplicationDbContext context, /*IProductRepository productReprository*/
            ICategoryRepository categoryReprositry, ISupplierRepository supplierRepository
            , IOrderAdminRepository orderReprository)
        {
            this.context = context;
            this.supplierRepository = supplierRepository;
            this.categoryRepository = categoryReprositry;
            this.supplierRepository = supplierRepository;
            this.orderRepository = orderReprository;
        }
        public IActionResult Index()
        {
            var orderreport = context.Orders.ToList();
           // ViewBag.productCount = productRepository.GetAll().Count();
            ViewBag.CategoryCount = categoryRepository.GetAll().Count();
            ViewBag.SupplierCount = supplierRepository.GetAll().Count();
            ViewBag.orderCount = orderRepository.GetAll().Count();
            return View(orderreport);
        }
    }
}
