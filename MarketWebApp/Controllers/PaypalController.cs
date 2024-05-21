using Microsoft.AspNetCore.Mvc;
using PayPal.Api;
using MarketWebApp.Models.Entity;
using MarketWebApp.Data;
using MarketWebApp;
using Microsoft.EntityFrameworkCore;

public class PaypalController : Controller
{
    private readonly ILogger<PaypalController> _logger;
    private IHttpContextAccessor httpContextAccessor;
    IConfiguration _configuration;
    private readonly ApplicationDbContext _context;

    public PaypalController(ILogger<PaypalController> logger, IHttpContextAccessor context, IConfiguration iconfiguration, ApplicationDbContext __context)
    {
        _logger = logger;
        httpContextAccessor = context;
        _configuration = iconfiguration;
        _context = __context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public ActionResult PaymentWithPaypal(string Cancel = null, string blogId = "", string PayerID = "", string guid = "")
    {
        //getting the apiContext  
        var ClientID = _configuration.GetValue<string>("PayPal:Key");
        var ClientSecret = _configuration.GetValue<string>("PayPal:Secret");
        var mode = _configuration.GetValue<string>("PayPal:mode");
        APIContext apiContext = PaypalConfiguration.GetAPIContext(ClientID, ClientSecret, mode);
        // apiContext.AccessToken="Bearer access_token$production$j27yms5fthzx9vzm$c123e8e154c510d70ad20e396dd28287";
        try
        {
            //A resource representing a Payer that funds a payment Payment Method as paypal  
            //Payer Id will be returned when payment proceeds or click to pay  
            string payerId = PayerID;
            if (string.IsNullOrEmpty(payerId))
            {
                //this section will be executed first because PayerID doesn't exist  
                //it is returned by the create function call of the payment class  
                // Creating a payment  
                // baseURL is the url on which paypal sendsback the data.  
                string baseURI = this.Request.Scheme + "://" + this.Request.Host + "/Paypal/PaymentWithPayPal?";
                //here we are generating guid for storing the paymentID received in session  
                //which will be used in the payment execution  
                var guidd = Convert.ToString((new Random()).Next(100000));
                guid = guidd;
                //CreatePayment function gives us the payment approval url  
                //on which payer is redirected for paypal account payment  
                var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid, blogId);
                //get links returned from paypal in response to Create function call  
                var links = createdPayment.links.GetEnumerator();
                string paypalRedirectUrl = null;
                while (links.MoveNext())
                {
                    Links lnk = links.Current;
                    if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                    {
                        //saving the payapalredirect URL to which user will be redirected for payment  
                        paypalRedirectUrl = lnk.href;
                    }
                }
                // saving the paymentID in the key guid  
                httpContextAccessor.HttpContext.Session.SetString("payment", createdPayment.id);
                return Redirect(paypalRedirectUrl);
            }
            else
            {
                // This function exectues after receving all parameters for the payment  

                var paymentId = httpContextAccessor.HttpContext.Session.GetString("payment");
                var executedPayment = ExecutePayment(apiContext, payerId, paymentId as string);
                //If executed payment failed then we will show payment failure message to user  
                if (executedPayment.state.ToLower() != "approved")
                {
                    return View("PaymentFailed");
                }
                var blogIds = executedPayment.transactions[0].item_list.items[0].sku;


                return View("PaymentSuccess");
            }
        }
        catch (Exception ex)
        {
            return View("PaymentFailed");
        }
        //on successful payment, show success page to user.  
        return View("SuccessView");
    }
    private PayPal.Api.Payment payment;
    private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
    {
        var paymentExecution = new PaymentExecution()
        {
            payer_id = payerId
        };
        this.payment = new Payment()
        {
            id = paymentId
        };
        return this.payment.Execute(apiContext, paymentExecution);
    }
    private Payment CreatePayment(APIContext apiContext, string redirectUrl, string blogId)
    {
        // Create item list and add item objects to it  
        var itemList = new ItemList()
        {
            items = new List<Item>()
        };

        // Adding Item Details like name, currency, price etc  
        // Adjust the price according to your requirements
        string userId = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var shoppingCart = _context.ShoppingCart
                                     .Include(sc => sc.ProductCarts)
                                         .ThenInclude(pc => pc.Product)
                                            .FirstOrDefault(sc => sc.ApplicationUserID == userId);
        itemList.items.AddRange(AddItems(shoppingCart));
        var details = new Details()
        {
            shipping = "30",
            subtotal =sum(shoppingCart).ToString(),
        };

        var payer = new Payer()
        {
            payment_method = "paypal"
        };

        // Configure Redirect Urls here with RedirectUrls object  
        var redirUrls = new RedirectUrls()
        {
            //cancel_url = redirectUrl + "&Cancel=true",
            cancel_url = this.Request.Scheme + "://" + this.Request.Host + Url.Action("Index", "ShoppingCart"),
            return_url = redirectUrl
        };

        var amount = new Amount()
        {
            currency = "USD", 
            total = (sum(shoppingCart) + 30).ToString(), 
            details = details,
        };

        var transactionList = new List<Transaction>();

        // Adding description about the transaction  
        transactionList.Add(new Transaction()
        {
            description = "Transaction description",
            invoice_number = Guid.NewGuid().ToString(), // Generate an Invoice No  
            amount = amount,
            item_list = itemList
        });

        this.payment = new Payment()
        {
            intent = "sale",
            payer = payer,
            transactions = transactionList,
            redirect_urls = redirUrls
        };

        // Create a payment using an APIContext  
        return this.payment.Create(apiContext);
    }
    private double sum(ShoppingCart shoppingCart)
    {
        Double sum = 0;
        foreach (var item in shoppingCart.ProductCarts)
        {
            sum += (item.Product.Price - (item.Product.Price * item.Product.Discount / 100)) * item.Quantity;
        }
        return sum;
    }
    private List<Item> AddItems(ShoppingCart shoppingCart)
    {
        List<Item> items = new List<Item>();
        foreach (var item in shoppingCart.ProductCarts)
        {
            items.Add(new Item()
            {
                name = item.Product.Name,
                currency = "USD", // Change currency to EGP for Egyptian pounds
                price = (item.Product.Price - (item.Product.Price * item.Product.Discount / 100)).ToString(), // Adjust the price accordingly
                quantity = item.Quantity.ToString(),
                sku = "asd"
            });

        }
        return items;
    }


}