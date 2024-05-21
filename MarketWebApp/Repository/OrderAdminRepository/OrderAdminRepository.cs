
using MarketWebApp.Data;
using MarketWebApp.Models.Entity;
using MarketWebApp.ViewModel.Order;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace MarketWebApp.Reprository.OrderReprository
{
    public class OrderAdminRepository : IOrderAdminRepository
    {
        private readonly ApplicationDbContext context;

        public OrderAdminRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public void Delete(int id)
        {
            var Order = GetOrder(id);
            context.Orders.Remove(Order);
            Save();
        }
        public IEnumerable<Order> GetAll()
        {
            return context.Orders.Include(b => b.OrderProducts).Include(c => c.applicationUser).ToList();
        }
        public Order GetOrder(int Id)
        {
            return context.Orders.Include(p => p.OrderProducts).SingleOrDefault(c => c.ID == Id);
        }
        public Order GetOrderWithSubOrders(int Id)
        {
            return context.Orders.Where(c => c.ID == Id).Include(c => c.OrderProducts).ThenInclude(p => p.Product).SingleOrDefault();

        }
        public void Insert(AddOrderViewModel addOrderViewModel)
        {
            var order = new Order();

            order.Date = addOrderViewModel.Date;
            order.LocationId = addOrderViewModel.LocationID;
            order.State = addOrderViewModel.State;
            order.ApplicationUserID = addOrderViewModel.applicationUserId;

            context.Orders.Add(order);
        }
        public void Save()
        {
            context.SaveChanges();
        }
        public void Update(EditOrderViewModel editOrderViewModel)
        {
            var order = GetOrder(editOrderViewModel.ID);
            order.Date = editOrderViewModel.Date;
            order.LocationId = editOrderViewModel.LocationID;
            order.State = editOrderViewModel.State;
            order.ApplicationUserID = editOrderViewModel.applicationUserId;
        }


        public Order InsertNew(AddOrderViewModel addOrderViewModel)
        {
            var order = new Order();
            order.Date = addOrderViewModel.Date;
            order.State = addOrderViewModel.State;
            order.LocationId = addOrderViewModel.LocationID;
            order.ApplicationUserID = addOrderViewModel.applicationUserId;
            context.Orders.Add(order);
            context.SaveChanges();
            return order;
        }
    }
}
