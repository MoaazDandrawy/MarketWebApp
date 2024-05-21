using MarketWebApp.Models.Entity;
using MarketWebApp.ViewModel;
using MarketWebApp.ViewModel.Supplier;
using System.Drawing.Drawing2D;

namespace MarketWebApp.Repository.SupplierRepository
{
    public interface ISupplierRepository
    {
        IEnumerable<Supplier> GetAll();
        IEnumerable<Supplier> SearchByName(string name);
        Supplier GetSupplier(int Id);
        Supplier GetSupplierWithProducts(int Id);
        void Insert(AddSupplierViewModel addSupplierViewModel);
        void Update(EditSupplierViewModel editSupplierViewModel);
        void Delete(int id);
        void Save();
        bool CheckSupplierExist(string Name);
        bool CheckSupplierExistEdit(string Name, int Id);

    }
}
