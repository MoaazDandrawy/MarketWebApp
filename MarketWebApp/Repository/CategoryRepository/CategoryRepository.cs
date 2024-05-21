using MarketWebApp.Data;
using MarketWebApp.Models.Entity;
using MarketWebApp.ViewModel;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;

namespace MarketWebApp.Reprository.CategoryReprositry
{
    public class CategoryRepository: ICategoryRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment webHostEnvironment;
        public CategoryRepository(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
        }
        public IEnumerable<Category> SearchByName(string name)
        {
            var categories = context.Categories
                .Where(s => s.Name.Contains(name))
                .ToList();

            if (categories == null || !categories.Any())
            {
                return Enumerable.Empty<Category>();
            }

            return categories;
        }
        public bool IsCategoryNameUnique(int categoryId, string categoryName)
        {
            // Check if there is any other category with the same name but a different ID
            return !context.Categories.Any(c => c.ID != categoryId && c.Name == categoryName);
        }
        public bool CheckCategoryExist(string Name)
        {
            return context.Categories.SingleOrDefault(c => c.Name.ToLower() == Name.ToLower()) == null;
        }

        public bool CheckCategoryExistEdit(string Name, int Id)
        {
            return context.Categories.SingleOrDefault(c => c.ID != Id && c.Name.ToLower() == Name.ToLower()) == null;
        }

        public void Delete(int id)
        {
            var category = GetCategory(id);

            context.Categories.Remove(category);
            string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "img/categories");
            string uniqueFileName = category.Img;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        public IEnumerable<Category> GetAll()
        {
            return context.Categories.ToList();
        }

        public Category GetCategory(int Id)
        {
            return context.Categories.SingleOrDefault(c => c.ID == Id);
        }

        public Category GetCategoryWithProducts(int Id)
        {
            return context.Categories.Where(c => c.ID == Id).Include(c => c.Products).SingleOrDefault();
        }

        public void Insert(AddCategoryViewModel addCategoryViewModel)
        {
            string uniqueFileName = UploadedFile(addCategoryViewModel.CategoryImage, addCategoryViewModel.Name);
            var category = new Category();
            category.Name = addCategoryViewModel.Name;
            category.Img = uniqueFileName;
            context.Categories.Add(category);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Update(EditCategoryViewModel editCategoryViewModel)
        {
            var category = GetCategory(editCategoryViewModel.ID);
            string uniqueFileName = UploadedFile(editCategoryViewModel.CategoryImage, editCategoryViewModel.Name);
            if (uniqueFileName is null)
            {
                // chang name 
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "img/categories");
                string oldPath = Path.Combine(uploadsFolder, category.Img);
                var extention = System.IO.Path.GetExtension(oldPath).ToString();
                var imageFullName = editCategoryViewModel.Name + "" + extention;
                string newPath = Path.Combine(uploadsFolder, imageFullName);
                category.Name = editCategoryViewModel.Name;
                category.Img = imageFullName;
                File.Move(oldPath, newPath);
            }
            else if (category.Name != editCategoryViewModel.Name && category.Img != uniqueFileName && uniqueFileName is not null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "img/categories");
                var filePath = Path.Combine(uploadsFolder, category.Img);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                category.Name = editCategoryViewModel.Name;
                category.Img = uniqueFileName;

            }
            else
            {
                //chang img 
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "img/categories");
                var filePath = Path.Combine(uploadsFolder, category.Img);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    editCategoryViewModel.CategoryImage.CopyTo(fileStream);
                }
                category.Name = editCategoryViewModel.Name;
                category.Img = uniqueFileName;
            }
        }
        public string UploadedFile(IFormFile model, string CatName)
        {
            string uniqueFileName = null;
            if (model != null)
            {

                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "img/categories");
                uniqueFileName = CatName + System.IO.Path.GetExtension(model.FileName).ToString();

                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
