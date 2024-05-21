using MarketWebApp.Models.Entity;
using MarketWebApp.ViewModel;

namespace MarketWebApp.Reprository.CategoryReprositry
{
    public interface ICategoryRepository
    {

        IEnumerable<Category> GetAll();
        IEnumerable<Category> SearchByName(string name);

        Category GetCategory(int Id);
        Category GetCategoryWithProducts(int Id);
        void Insert(AddCategoryViewModel addCategoryViewModel);
        void Update(EditCategoryViewModel editCategoryViewModel);
        void Delete(int id);
        void Save();
        string UploadedFile(IFormFile model, string CatName);
        bool CheckCategoryExist(string Name);
        bool CheckCategoryExistEdit(string Name, int Id);
        bool IsCategoryNameUnique(int categoryId, string categoryName);
    }
}
