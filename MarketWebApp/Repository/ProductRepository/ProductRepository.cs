using MarketWebApp.Data;
using MarketWebApp.Models.Entity;
using MarketWebApp.ViewModel.Product;
using Microsoft.EntityFrameworkCore;

namespace MarketWebApp.Repository.ProductRepository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ProductRepository(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
        }
        public bool CheckProductExist(string Name)
        {
            return context.Products.SingleOrDefault(p => p.Name.ToLower() == Name.ToLower()) == null;
        }
        public IEnumerable<Product> SearchByName(string name)
        {
            var products = context.Products
                .Where(s => s.Name.Contains(name))
                .ToList();

            if (products == null || !products.Any())
            {
                return Enumerable.Empty<Product>();
            }

            return products;
        }
        public bool CheckProductExistEdit(string Name, int Id)
        {
            return context.Products.SingleOrDefault(p => p.ID != Id && p.Name.ToLower() == Name.ToLower()) == null;
        }
        public bool IsProductUnique(int productId, string name, int supplierId, int categoryId)
        {
            // Implement the logic to check if a product with the same name exists for the same supplier and category
            return !context.Products.Any(p =>
                p.ID != productId && p.Name==name &&
                p.SupplierId == supplierId && p.CategoryId == categoryId);
        }
        public void Delete(int id)
        {
            var product = GetProduct(id);

            context.Products.Remove(product);
            string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "img/product");
            string uniqueFileName = product.Img;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            Save();
        }

        public IEnumerable<Product> GetAll()
        {
            return context.Products.Include(b => b.Supplier).Include(c => c.Category).ToList();
        }

        public Product GetProduct(int Id)
        {
            return context.Products.SingleOrDefault(c => c.ID == Id);
        }

        public Product GetProductWithOrders(int Id)
        {
            return context.Products.Where(c => c.ID == Id).Include(c => c.OrderProducts).SingleOrDefault();
        }

        public void Insert(AddProductViewModel addProductViewModel)
        {
            string uniqueFileName = UploadedFile(addProductViewModel.Img, addProductViewModel.Name);
            var product = new Product();

            product.Name = addProductViewModel.Name;
            product.Price = addProductViewModel.Price;
            product.Discount = addProductViewModel.Discount;
            product.Detail = addProductViewModel.Detail;
            product.Unit = addProductViewModel.Unit;
            product.CategoryId = addProductViewModel.CategoryID;
            product.Stock = addProductViewModel.Stock;
            product.SupplierId = addProductViewModel.SupplierId;
            product.Img = uniqueFileName;
            context.Products.Add(product);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Update(EditProductViewModel editProductViewModel)
        {
            string uniqueFileName = UploadedFile(editProductViewModel.ProductImage, editProductViewModel.Name);
            var product = GetProduct(editProductViewModel.ID);

            if (uniqueFileName is null)
            {
                // chang name 
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "img/product");
                string oldPath = Path.Combine(uploadsFolder, product.Img);
                var extention = System.IO.Path.GetExtension(oldPath).ToString();
                var imageFullName = editProductViewModel.Name + "" + extention;
                string newPath = Path.Combine(uploadsFolder, imageFullName);
                product.Name = editProductViewModel.Name;
                product.Img = imageFullName;
                product.Discount = editProductViewModel.Discount;
                product.Detail=editProductViewModel.Detail;
                product.Unit = editProductViewModel.Unit;
                product.Price = editProductViewModel.Price;
                product.Stock = editProductViewModel.Stock;
                product.CategoryId = editProductViewModel.CategoryID;
                product.SupplierId = editProductViewModel.SupplierId;
                File.Move(oldPath, newPath);
            }
            else if (product.Name != editProductViewModel.Name && product.Img != uniqueFileName && uniqueFileName is not null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "img/product");
                var filePath = Path.Combine(uploadsFolder, product.Img);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                product.Name = editProductViewModel.Name;
                product.Img = uniqueFileName;
                product.Discount = editProductViewModel.Discount;
                product.Price = editProductViewModel.Price;
                product.Detail = editProductViewModel.Detail;
                product.Unit = editProductViewModel.Unit;
                product.Stock = editProductViewModel.Stock;
                product.CategoryId = editProductViewModel.CategoryID;
                product.SupplierId = editProductViewModel.SupplierId;

            }
            else
            {
                //chang img 
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "img/product");
                var filePath = Path.Combine(uploadsFolder, product.Img);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    editProductViewModel.ProductImage.CopyTo(fileStream);
                }

                product.Name = editProductViewModel.Name;
                product.Img = uniqueFileName;
                product.Discount = editProductViewModel.Discount;
                product.Price = editProductViewModel.Price;
                product.Detail = editProductViewModel.Detail;
                product.Unit = editProductViewModel.Unit;
                product.Stock = editProductViewModel.Stock;
                product.CategoryId = editProductViewModel.CategoryID;
                product.SupplierId = editProductViewModel.SupplierId;
            }
            Save();
        }

        public string UploadedFile(IFormFile model, string ProdName)
        {
            string uniqueFileName = null;
            if (model != null)
            {

                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "img/product");
                uniqueFileName = ProdName + System.IO.Path.GetExtension(model.FileName).ToString();

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        public List<Product> GetProducts(List<int>? prodlist)
        {

            var products = new List<Product>();
            foreach (int prod in prodlist)
            {
                GetProduct(prod);
                products.Add(GetProduct(prod));
            }
            return products;
        }
    }
}
