using MarketWebApp.Models.Entity;
using MarketWebApp.ViewModel.OrderProduct;

namespace MarketWebApp.Reprository
{
    public interface IOrderProductRepository
    {
        IEnumerable<OrderProduct> GetAll();
        OrderProduct GetOrderProduct(int Id);
        IEnumerable<OrderProduct> GetOrderProductForOrder(int Id);
        IEnumerable<OrderProduct> GetOrderProductForProduct(int Id);

        void Insert(AddOrderProductViewModel addOrderProductViewModel);
        void Update(EditOrderProductViewModel editOrderProductViewModel);
        void Delete(int id);
        void Save();
    }
}
