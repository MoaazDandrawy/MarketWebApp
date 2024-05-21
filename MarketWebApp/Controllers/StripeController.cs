using MarketWebApp.Models.Entity;
using MarketWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using Stripe;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using MarketWebApp.Models;
using MarketWebApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.CSharp;
using NuGet.Protocol;


namespace MarketWebApp.Controllers
{
    public class StripeController : Controller
    {
        private readonly StripeSettings _stripeSettings;
        private readonly ApplicationDbContext context;
        public StripeController(IOptions<StripeSettings> stripeSettings, ApplicationDbContext _context)
        {
            _stripeSettings = stripeSettings.Value;
            context = _context;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult CreateCheckoutSession()
        {
            string userId = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var shoppingCart = context.ShoppingCart
                                         .Include(sc => sc.ProductCarts)
                                             .ThenInclude(pc => pc.Product)
                                                .FirstOrDefault(sc => sc.ApplicationUserID == userId);
            PlaceOrderViewModel Order= new PlaceOrderViewModel { ShoppingCart = shoppingCart, Locations = new List<Location>(), SelectedLocationId = 1 };
            var currency = "EGP";
            var successUrl = this.Request.Scheme + "://" + this.Request.Host + Url.Action("ConfirmOrder", "Order", Order);
            var cancelUrl = this.Request.Scheme + "://" + this.Request.Host + Url.Action("Index", "ShoppingCart");
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;


            var lineItems = new List<SessionLineItemOptions>();

            // Loop through each product in the shopping cart and add it to line items
            foreach (var productCart in shoppingCart.ProductCarts)
            {
                lineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = currency,
                        UnitAmount = Convert.ToInt32(productCart.Product.Price - (productCart.Product.Price * productCart.Product.Discount / 100)) * 100,  // Amount in smallest currency unit (e.g., cents)
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = productCart.Product.Name,
                        }
                    },
                    Quantity = productCart.Quantity,
                }) ; 

            }
            lineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = currency,
                    UnitAmount = 3000,  // 3000 cents = 30 EGP
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Shipping Tax"
                    }
                },
                Quantity = 1
            });

            // Create the session options with the line items
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
    {
        "card",
    },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl
            };

            var service = new SessionService();
            var session = service.Create(options);


            return Redirect(session.Url);
        }

        public async Task<IActionResult> success()
        {

            return View("Index");
        }

        public IActionResult cancel()
        {
            return View("Index");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
