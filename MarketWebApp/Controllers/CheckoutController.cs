using MarketWebApp.Data;
using MarketWebApp.Models.Entity;
using Microsoft.AspNetCore.Mvc;

namespace MarketWebApp.Controllers
{
    public class CheckoutController : BaseController
    {

        private readonly ApplicationDbContext _context;

        public CheckoutController(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }


       
    }
}
