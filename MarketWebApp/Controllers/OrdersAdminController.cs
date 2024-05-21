using MarketWebApp.Reprository;
using MarketWebApp.Reprository.OrderReprository;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace MarketWebApp.Controllers
{
  //  [Authorize(Roles = "Admin")]
    public class OrdersAdminController : Controller
    {
        private readonly IOrderAdminRepository reprositry;
        private readonly  IOrderProductRepository reprositrySubOrder;

        public OrdersAdminController(IOrderAdminRepository reprositry, IOrderProductRepository reprositrySubOrder)
        {
            this.reprositry = reprositry;
            this.reprositrySubOrder = reprositrySubOrder;
        }

        // GET: OrderController
        public ActionResult Index()
        {
            return View(this.reprositry.GetAll());
        }

        // GET: OrderController/Details/5
        public ActionResult Details(int Id)
        {
            return View(this.reprositrySubOrder.GetOrderProductForOrder(Id));
        }

        // GET: OrderController/Create
        public ActionResult Create()
        {
            return View();
        }

        public IActionResult GenerateReport()
        {
            var Products = reprositry.GetAll();
            ExcelPackage.LicenseContext = LicenseContext.Commercial; // For commercial licenses
            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // For non-commercial licenses

            using (var package = new ExcelPackage())
            {

                var worksheet = package.Workbook.Worksheets.Add("Products");


                var headerStyle = worksheet.Cells[1, 1, 1, 7].Style;
                headerStyle.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                headerStyle.Fill.BackgroundColor.SetColor(System.Drawing.Color.Black);
                headerStyle.Font.Color.SetColor(System.Drawing.Color.White);

                worksheet.Cells[1, 1].Value = "Order ID";
                worksheet.Cells[1, 2].Value = "Order Date";
                worksheet.Cells[1, 3].Value = "Cost";
                worksheet.Cells[1, 4].Value = "State";
                worksheet.Cells[1, 5].Value = "User";



                worksheet.Cells[worksheet.Dimension.Address].AutoFilter = true;


                worksheet.Column(1).Width = 15;
                worksheet.Column(2).Width = 20;
                worksheet.Column(3).Width = 15;
                worksheet.Column(4).Width = 20;
                worksheet.Column(5).Width = 20;


                int row = 2;
                foreach (var product in Products)
                {
                    worksheet.Cells[row, 1].Value = product.ID.ToString();
                    worksheet.Cells[row, 2].Value = product.Date.ToString("dd/MM/yyyy");
                    worksheet.Cells[row, 5].Value = product.OrderProducts;
                    worksheet.Cells[row, 6].Value = product.State;
                    worksheet.Cells[row, 7].Value = product.ApplicationUserID;
                    row++;
                }


                

                var excelBytes = package.GetAsByteArray();
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Orders Report.xlsx");
            }
        }
    }
}
