using MarketWebApp.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using DinkToPdf;
using MarketWebApp.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using DinkToPdf.Contracts;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using MarketWebApp.ViewModel;
using Microsoft.AspNetCore.Hosting;


    namespace MarketWebApp.Controllers
    {
        public class BillingController : Controller
        {
            private readonly ApplicationDbContext _context;
            private readonly IConverter _pdfConverter;
            private readonly IWebHostEnvironment _hostingEnvironment;

            public BillingController(ApplicationDbContext context, IConverter pdfConverter, IWebHostEnvironment hostingEnvironment)
            {
                _context = context;
                _pdfConverter = pdfConverter;
                _hostingEnvironment = hostingEnvironment;
            }

  

        public IActionResult PrintOrder(int Id)
        {
            var order = _context.Orders.Include(o => o.applicationUser)
                                       .Include(o => o.OrderProducts)
                                           .ThenInclude(op => op.Product)
                                       .FirstOrDefault(o => o.ID == Id);

            if (order == null)
            {
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction("Index", "ConfirmOrder");
            }

            var billingViewModel = new BillingViewModel
            {
                CustomerName = order.applicationUser.UserName,
                Phone = order.applicationUser.PhoneNumber,
                BillingDetails = order.OrderProducts.Select(op => new BillingDetailViewModel
                {
                    ProductName = op.Product.Name,
                    Quantity = op.Quantity,
                    UnitPrice = op.Product.Price,
                    Discount = op.Product.Discount,
                    TotalWithDiscount = op.Price - (op.Price * op.Product.Discount / 100),
                    Total = op.Quantity * (op.Price - (op.Price * op.Product.Discount / 100))
                }).ToList(),
                TotalAmount = order.OrderProducts.Sum(op => op.Quantity * op.Product.Price)
            };

            var htmlContent = RenderViewToStringAsync("Billing", billingViewModel).GetAwaiter().GetResult();
            var pdfBytes = _pdfConverter.Convert(new HtmlToPdfDocument()
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
            });

            return File(pdfBytes, "application/pdf", $"SalesReport_{order.ID}.pdf");
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