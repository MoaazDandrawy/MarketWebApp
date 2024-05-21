using MarketWebApp.Data;
using MarketWebApp.Models.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Net;
using MarketWebApp.ViewModel;

namespace MarketWebApp.Controllers
{
    public class HomeController : BaseController
    {
        ApplicationDbContext context;



        public HomeController(ApplicationDbContext _context) : base(_context) {
            context = _context;
        }


        public IActionResult Index(int page = 1)
        {
            int pageSize = 8; 
            var totalItems = context.Products.Where(Product => Product.Discount >= 0&&Product.Stock>0).Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var products = context.Products
                                    .Where(Product => Product.Discount >= 0 && Product.Stock > 0)
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();

            var viewModel = new 
            {
                Products = products,
                CurrentPage = page,
                TotalPages = totalPages
            };
           
            return View(viewModel);
        }


        [HttpPost]
        [HttpGet]
        public IActionResult Search(string searchString, int page = 1)
        {
            int pageSize = 8; // Number of items per page
            var totalItems = context.Products.Where(p => p.Name.Contains(searchString) && p.Stock > 0).Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var products = context.Products
                                    .Where(p => p.Name.Contains(searchString) && p.Stock > 0)
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();

            var viewModel = new 
            {
                Products = products,
                CurrentPage = page,
                searchString=searchString,
                TotalPages = totalPages
            };

            return View(viewModel);
        }

        public ActionResult Contact()
        {
            return View();
        } 

        public ActionResult Details(int id)
        {
            return View(context.Products.Find(id));
        }


        [HttpPost]
        public ActionResult SubmitContactForm(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
               
                SendReviewEmail(model.Email);

                return RedirectToAction("Contact", "Home");
            }
            else
            {
                return View("Contact", model); 
            }
        }
        private void SendReviewEmail(string email)
        {
            
            var fromAddress = new MailAddress("mohamed.b97.2020@gmail.com", "Se7teenW3fia");
            var toAddress = new MailAddress(email);
            const string subject = "We'd love to hear your feedback!";
            const string body = "Thank you for contacting us! We value your feedback. Please take a moment to review your experience with us.";

            
            using (var smtp = new SmtpClient())
            {
                var credentials = new NetworkCredential("mohamed.b97.2020@gmail.com", "zhvpbtzyffrppfvj"); 
                smtp.Credentials = credentials;
                smtp.Host = "smtp.gmail.com"; 
                smtp.Port = 587; 
                smtp.EnableSsl = true; 

                var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                };
                smtp.Send(message);
            }

          
        }

    }
}
