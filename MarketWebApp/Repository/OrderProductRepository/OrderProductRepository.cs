using MarketWebApp.Data;
using MarketWebApp.Models.Entity;
using MarketWebApp.ViewModel.OrderProduct;
using Microsoft.EntityFrameworkCore;

namespace MarketWebApp.Reprository
{
    public class OrderProductRepository : IOrderProductRepository
    {
        private readonly ApplicationDbContext context;

        public OrderProductRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void Delete(int id)
        {
            var orderProduct = GetOrderProduct(id);
            context.OrderProduct.Remove(orderProduct);

        }

        public IEnumerable<OrderProduct> GetAll()
        {
            return context.OrderProduct.Include(c => c.Product).ToList();

        }

        public OrderProduct GetOrderProduct(int Id)
        {
            return context.OrderProduct.SingleOrDefault(c => c.Id == Id);
        }

        public IEnumerable<OrderProduct> GetOrderProductForOrder(int Id)
        {
            return context.OrderProduct.Where(c => c.OrderId == Id).Include(c => c.Product).ToList();
        }

        public IEnumerable<OrderProduct> GetOrderProductForProduct(int Id)
        {
            return context.OrderProduct.Where(c => c.ProductId == Id).Include(c => c.Product).ToList();
        }

       
        public void Insert(AddOrderProductViewModel addSubOrderViewModel)
        {
            var Suborder = new OrderProduct();
            Suborder.Quantity = addSubOrderViewModel.Quantity;
            Suborder.ProductId = addSubOrderViewModel.ProductId;
            Suborder.OrderId = addSubOrderViewModel.OrderId;
            context.OrderProduct.Add(Suborder);
        }

        

        public void Save()
        {
            context.SaveChanges();
        }

        public void Update(EditOrderProductViewModel editSubOrderViewModel)
        {
            var Suborder = GetOrderProduct(editSubOrderViewModel.Id);
            Suborder.Quantity = editSubOrderViewModel.Quantity;
            Suborder.ProductId = editSubOrderViewModel.ProductId;
            Suborder.OrderId = editSubOrderViewModel.OrderId;
        }
    }
}
