using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using MarketWebApp.Data;
using MarketWebApp.Repository.ProductRepository;
using MarketWebApp.Repository.SupplierRepository;
using MarketWebApp.Reprository.CategoryReprositry;
using MarketWebApp.Reprository.OrderReprository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace MarketWebApp.Controllers
{
    public class ReportPDFController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConverter _pdfConverter;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IOrderAdminRepository _orderRepository;

        public ReportPDFController(IConverter pdfConverter, ApplicationDbContext context,
            IProductRepository productRepository, ICategoryRepository categoryRepository,
            ISupplierRepository supplierRepository, IOrderAdminRepository orderRepository)
        {
            _context = context;
            _pdfConverter = pdfConverter;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _supplierRepository = supplierRepository;
            _orderRepository = orderRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GeneratePDF(string InvoiceNo)
        {
            ViewBag.productCount = _productRepository.GetAll().Count();
            ViewBag.CategoryCount = _categoryRepository.GetAll().Count();
            ViewBag.SupplierCount = _supplierRepository.GetAll().Count();
            ViewBag.orderCount = _orderRepository.GetAll().Count();

            var monthOrders = _context.Orders
                .Where(c => c.Date.Month == DateTime.Now.Month)
                .ToList();

            float monthCost = monthOrders
                .SelectMany(o => o.OrderProducts)
                .Sum(p => p.Price);

            ViewBag.monthOrders = _context.Orders.Where(c => c.Date.Month == DateTime.Now.Month).Count();
            ViewBag.yearOrders = _context.Orders.Where(c => c.Date.Year == DateTime.Now.Year).Count();

            // Render HTML content from a view to string
            var htmlContent = await RenderViewToStringAsync("GeneratePDF", null);

            // Convert HTML content to PDF
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                },
                Objects = {
                    new ObjectSettings() {
                        HtmlContent = htmlContent
                    }
                }
            };

            // Generate PDF bytes
            var pdfBytes = _pdfConverter.Convert(pdf);

            // Return PDF as a file
            return File(pdfBytes, "application/pdf", $"SalesReport_{InvoiceNo}.pdf");
        }
        public async Task<IActionResult> GenerateBillingPDF(string InvoiceNo)
        {
            ViewBag.productCount = _productRepository.GetAll().Count();
            ViewBag.CategoryCount = _categoryRepository.GetAll().Count();
            ViewBag.SupplierCount = _supplierRepository.GetAll().Count();
            ViewBag.orderCount = _orderRepository.GetAll().Count();

            var monthOrders = _context.Orders
                .Where(c => c.Date.Month == DateTime.Now.Month)
                .ToList();

            float monthCost = monthOrders
                .SelectMany(o => o.OrderProducts)
                .Sum(p => p.Price);

            ViewBag.monthOrders = _context.Orders.Where(c => c.Date.Month == DateTime.Now.Month).Count();
            ViewBag.yearOrders = _context.Orders.Where(c => c.Date.Year == DateTime.Now.Year).Count();

            // Render HTML content from a view to string
            var htmlContent = await RenderViewToStringAsync("GeneratePDF", null);

            // Convert HTML content to PDF
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                },
                Objects = {
                    new ObjectSettings() {
                        HtmlContent = htmlContent
                    }
                }
            };

            // Generate PDF bytes
            var pdfBytes = _pdfConverter.Convert(pdf);

            // Return PDF as a file
            return File(pdfBytes, "application/pdf", $"SalesReport_{InvoiceNo}.pdf");
        }

        public async Task<string> RenderViewToStringAsync(string viewName, object model)
        {
            ViewData.Model = model;

            using (var writer = new StringWriter())
            {
                var viewEngineResult = HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;

                var viewContext = new ViewContext(
                    ControllerContext,
                    viewEngineResult.FindView(ControllerContext, viewName, true).View,
                    ViewData,
                    TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                await viewContext.View.RenderAsync(viewContext);

                return writer.GetStringBuilder().ToString();
            }
        }
    }
}
