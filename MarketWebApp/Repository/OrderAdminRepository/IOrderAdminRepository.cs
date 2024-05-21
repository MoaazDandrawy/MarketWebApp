

using MarketWebApp.Models.Entity;
using MarketWebApp.ViewModel.Order;

namespace MarketWebApp.Reprository.OrderReprository
{
    public interface IOrderAdminRepository
    {
        IEnumerable<Order> GetAll();
        Order GetOrder(int Id);
        Order GetOrderWithSubOrders(int Id);
        void Insert(AddOrderViewModel addOrderViewModel);
        void Update(EditOrderViewModel editOrderViewModel);
        void Delete(int id);
        void Save();
        Order InsertNew(AddOrderViewModel addOrderViewModel);
    }
}
