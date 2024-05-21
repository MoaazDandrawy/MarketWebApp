using MarketWebApp.Models.Entity;
using MarketWebApp.ViewModel.Product;

namespace MarketWebApp.Repository.ProductRepository
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetAll();
        IEnumerable<Product> SearchByName(string name);

        Product GetProduct(int Id);
        Product GetProductWithOrders(int Id);
        void Insert(AddProductViewModel addProductViewModel);
        void Update(EditProductViewModel editProductViewModel);
        void Delete(int id);
        void Save();
        string UploadedFile(IFormFile model, string ProdName);
        bool CheckProductExist(string Name);
        bool CheckProductExistEdit(string Name, int Id);
        bool IsProductUnique(int productId, string name, int supplierId, int categoryId);

        List<Product> GetProducts(List<int>? _productsSelected);

    }
}
