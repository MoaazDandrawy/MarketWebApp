using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MarketWebApp.Reprository.CategoryReprositry;
using MarketWebApp.ViewModel;
using MarketWebApp.Repository.SupplierRepository;

namespace MarketWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryRepository repository;

        public CategoriesController(ICategoryRepository repository)
        {
            this.repository = repository;
        }

        // GET: Categories
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            ViewBag.PageCount = (int)Math.Ceiling((decimal)repository.GetAll().Count() / 5m);
            return View(repository.GetAll());
        }

        public IActionResult GetCategories(int pageNumber, int pageSize = 5, string searchQuery = "")
        {
            // Check if there's a search query
            if (!string.IsNullOrEmpty(searchQuery))
            {
                var categories = repository.SearchByName(searchQuery)
                    .OrderBy(p => p.ID)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return PartialView("_CategoryTable", categories);
            }
            else
            {
                var categories = repository.GetAll()
                    .OrderBy(p => p.ID)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return PartialView("_CategoryTable", categories);
            }
        }

        [Authorize(Roles = "Admin")]
        // GET: Categories/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(AddCategoryViewModel categoryViewModel)
        {
            if (ModelState.IsValid)
            {
                string categoryNameToLower = categoryViewModel.Name.ToLower(); // Convert input name to lowercase

                if (repository.GetAll().Any(c => c.Name.ToLower() == categoryNameToLower))
                {
                    ModelState.AddModelError("", "Department with the same name already exists.");
                    return View(categoryViewModel);
                }

                try
                {
                    repository.Insert(categoryViewModel);
                    repository.Save();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(categoryViewModel);
                }
            }
            else
            {
                return View(categoryViewModel);
            }
        }
        [Authorize(Roles = "Admin")]
        public IActionResult CheckCategoryExist(string Name)
        {
            if (repository.CheckCategoryExist(Name))
                return Json(true);
            else
                return Json(false);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult CheckCategoryExistEdit(string Name, int Id)
        {
            if (repository.CheckCategoryExistEdit(Name, Id))
                return Json(true);
            else
                return Json(false);
        }

        // GET: Categories/Edit/5

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int Id)
        {
            var category = repository.GetCategory(Id);
            EditCategoryViewModel categoryViewModel = new EditCategoryViewModel();
            categoryViewModel.ID = category.ID;
            categoryViewModel.Name = category.Name;
            categoryViewModel.Image = category.Img;
            return View(categoryViewModel);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(EditCategoryViewModel categoryViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string categoryNameToLower = categoryViewModel.Name.ToLower(); // Convert input name to lowercase

                    if (!repository.IsCategoryNameUnique(categoryViewModel.ID, categoryNameToLower))
                    {
                        ModelState.AddModelError("Name", "Department name already exists.");
                        return View(categoryViewModel);
                    }

                    repository.Update(categoryViewModel);
                    repository.Save();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(categoryViewModel);
                }
            }
            else
            {
                return View(categoryViewModel);
            }
        }


        // GET: Categories/Delete/5
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int Id)
        {
            var data = repository.GetCategoryWithProducts(Id);
            ViewBag.flag = data.Products.Count > 0 ? true : false;
            return View(data);
        }

        // POST: Categories/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult ConfirmDelete(int Id)
        {

            if (ModelState.IsValid)
            {
                repository.Delete(Id);
                repository.Save();
                return RedirectToAction("Index");
            }
            return View("Delete");
        }
        

    
    }
}
