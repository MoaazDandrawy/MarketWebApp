using BoldReports.Processing.ObjectModels;
using MarketWebApp.Data;
using MarketWebApp.Models.Entity;
using MarketWebApp.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MarketWebApp.Controllers
{
    public class ConfirmOrderController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public ConfirmOrderController(ApplicationDbContext context, EmailService emailService) : base(context)
        {
            _context = context;
            _emailService = emailService;
        }

        public IActionResult GetConfirmOrder(int pageNumber, int pageSize = 5)
        {
            var orders = GetAll()
                .OrderByDescending(o => o.Orders.FirstOrDefault()?.Date)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return PartialView("_ordersTable", orders);
        }
        public IEnumerable<UserOrdersViewModel> GetAll()
        {
            var userOrdersViewModels = _context.Orders
                .Where(order => order.State == "Pending")
                .Include(order => order.OrderProducts)
                    .ThenInclude(orderProduct => orderProduct.Product)
                .Include(order => order.applicationUser)
                .Select(order => new UserOrdersViewModel
                {
                    User = order.applicationUser,
                    Orders = new List<Order> { order }
                })
                .ToList();

            return userOrdersViewModels;
        }

        [Authorize(Roles = "Admin,Cashier")]
        public IActionResult Index()
        {
            var usersWithOrders = _context.Users
                .Include(u => u.Orders.Where(o => o.State == "Pending"))
                    .ThenInclude(o => o.OrderProducts)
                        .ThenInclude(op => op.Product)
                .ToList();

            if (usersWithOrders == null || usersWithOrders.Count == 0)
            {
                return NotFound();
            }

            int pageCount = (int)Math.Ceiling((decimal)usersWithOrders.SelectMany(u => u.Orders).Count() / 5m);
            var viewModelList = new List<UserOrdersViewModel>();

            foreach (var user in usersWithOrders)
            {
                var userOrdersViewModel = new UserOrdersViewModel
                {
                    User = user,
                    Orders = user.Orders.ToList()
                };
                viewModelList.Add(userOrdersViewModel);
            }
            ViewBag.PageCount = pageCount;

            return View(viewModelList);
        }


        [HttpGet]
        [Authorize(Roles = "Admin,Cashier")]

        public IActionResult AcceptOrder(int Id)
        {
            var order = _context.Orders.Include(o => o.applicationUser).Include(o => o.OrderProducts).ThenInclude(op => op.Product)
                               .FirstOrDefault(o => o.ID == Id);

            if (order != null)
            {
                if (order.State != "Shipping" && order.OrderProducts.Any())
                {
                    order.State = "Shipping";

                    if (order.State == "Shipping")
                    {
                        foreach (var orderProduct in order.OrderProducts)
                        {
                            var product = orderProduct.Product;

                            if (orderProduct.Quantity <= 0)
                            {
                                TempData["ErrorMessage"] = "Quantity cannot be 0.";
                                return RedirectToAction("Index", "ConfirmOrder");
                            }
                            else if (orderProduct.Quantity > product.Stock)
                            {
                                TempData["ErrorMessage"] = $"Insufficient stock for product: {product.Name}. Available stock: {product.Stock}.";
                                return RedirectToAction("Index", "ConfirmOrder");
                            }

                            product.Stock -= orderProduct.Quantity;

                            _context.Products.Update(product);
                        }

                        _context.SaveChanges();
                    }
                    _emailService.SendEmail(order.applicationUser.UserName, "Order Accepted", "Dear, Your order has been accepted and is now being shipped.");
                    return RedirectToAction("PrintOrder","Billing", new { Id = Id });
                }
                else
                {
                    TempData["ErrorMessage"] = "Order is already shipping or no products in the order.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Order not found.";
            }

            return RedirectToAction("Index", "ConfirmOrder");
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Cashier")]

        public IActionResult RejectOrder(int Id)
        {
            var order = _context.Orders.Include(o => o.applicationUser).Include(o => o.OrderProducts).ThenInclude(op => op.Product)
                                          .FirstOrDefault(o => o.ID == Id);
            if (order != null)
            {
                // Update the order state to "Confirmed"
                order.State = "Rejected";
                _context.SaveChanges();
                _emailService.SendEmail(order.applicationUser.UserName, "Order Rejected", "Dear , We regret to inform you that your order has been rejected.");
            }

            return RedirectToAction("Index", "ConfirmOrder");
        }

        [Authorize(Roles = "Admin,Cashier")]

        public IActionResult DetailsOfOrder(int id)
        {
            var orderitem = _context.OrderProduct.Include(s => s.Order).Include(p => p.Product).Where(o => o.OrderId == id).ToList();

            return View(orderitem);
        }




        public IActionResult GetUserHistory(int pageNumber, int pageSize = 1)
        {
            var orders = GetAllHistory()
                .OrderByDescending(o => o.Orders.FirstOrDefault()?.Date)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return PartialView("_userHistory", orders);
        }
        public IEnumerable<UserOrdersViewModel> GetAllHistory()
        {
            var usersWithOrderHistory = _context.Users
                .Include(u => u.Orders.Where(o => o.State != "Pending"))
                    .ThenInclude(o => o.OrderProducts)
                        .ThenInclude(op => op.Product)
                .ToList()
                .Select(user => new UserOrdersViewModel
                {
                    User = user,
                    Orders = user.Orders.ToList()
                })
                .ToList();

            return usersWithOrderHistory;
        }



        public IActionResult UserHistory()
        {
            var usersWithOrderHistory = _context.Users
                .Include(u => u.Orders.Where(o => o.State != "Pending"))
                    .ThenInclude(o => o.OrderProducts)
                        .ThenInclude(op => op.Product)
                .ToList();

            if (usersWithOrderHistory == null || usersWithOrderHistory.Count == 0)
            {
                return NotFound();
            }

            var viewModelList = new List<UserOrdersViewModel>();

            foreach (var user in usersWithOrderHistory)
            {
                var userOrdersViewModel = new UserOrdersViewModel
                {
                    User = user,
                    Orders = user.Orders.ToList()
                };
                viewModelList.Add(userOrdersViewModel);
            }

            ViewBag.PageCount = (int)Math.Ceiling((decimal)usersWithOrderHistory.SelectMany(u => u.Orders).Count() / 5m);
            return View(viewModelList);
        }
    }
}
