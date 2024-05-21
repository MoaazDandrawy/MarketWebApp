using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MarketWebApp.Data;
using MarketWebApp.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using MarketWebApp.Reprository.CategoryReprositry;
using MarketWebApp.Repository.ProductRepository;
using NuGet.Protocol.Core.Types;
using MarketWebApp.Repository.SupplierRepository;
using MarketWebApp.ViewModel.Product;
using MarketWebApp.ViewModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MarketWebApp.Controllers
{
    [Authorize(Roles = "Admin")]

    public class ProductsController : Controller
    {
        private readonly IProductRepository repository;
        private readonly ICategoryRepository repositorycaty;
        private readonly ISupplierRepository repositorySupplier;
        private readonly ApplicationDbContext context;

        public ProductsController(IProductRepository repository, ICategoryRepository repositorycaty ,ISupplierRepository repositorySupplier ,ApplicationDbContext _context)
        {
            this.repository = repository;
            this.repositorycaty = repositorycaty;
            this.repositorySupplier = repositorySupplier;
            this.context = _context;

        }

        // GET: Products

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            ViewBag.PageCount = (int)Math.Ceiling((decimal)repository.GetAll().Count() / 5m);
            return View(this.repository.GetAll());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult GetProducts(int pageNumber, int pageSize = 5, string searchQuery = "")
        {
            // Check if there's a search query
            if (!string.IsNullOrEmpty(searchQuery))
            {
                var products = repository.SearchByName(searchQuery)
                    .OrderBy(p => p.ID)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return PartialView("_ProductTable", products);
            }
            else
            {
                var products = repository.GetAll()
                    .OrderBy(p => p.ID)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return PartialView("_ProductTable", products);
            }
        }

        // GET: Products/Create

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(repositorycaty.GetAll(), "ID", "Name");
            ViewData["SupplierId"] = new SelectList(repositorySupplier.GetAll(), "ID", "Name");
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]

        [Authorize(Roles = "Admin")]
        public IActionResult Create(AddProductViewModel addProductViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string productNameToLower = addProductViewModel.Name.ToLower(); // Convert input name to lowercase

                    // Check if a product with the same name already exists for the same supplier and category
                    bool productNameExists = repository.GetAll()
                        .Any(p => p.Name.ToLower() == productNameToLower &&
                                   p.SupplierId == addProductViewModel.SupplierId &&
                                   p.CategoryId == addProductViewModel.CategoryID);

                    if (productNameExists)
                    {
                        ModelState.AddModelError("Name", "Product name already exists for the same supplier and category.");
                        PopulateDropdowns();
                        return View(addProductViewModel);
                    }

                    // Insert the new product into the repository
                    repository.Insert(addProductViewModel);
                    repository.Save();

                    // Redirect to the index action after successful creation
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    PopulateDropdowns();
                    return View(addProductViewModel);
                }
            }
            else
            {
                // If the model state is not valid, return the view with the model to display validation errors
                return View(addProductViewModel);
            }
        }



        [Authorize(Roles = "Admin")]
        private void PopulateDropdowns()
        {
            // Populate dropdown lists for categories and suppliers
            ViewData["CategoryID"] = new SelectList(repositorycaty.GetAll(), "ID", "Name");
            ViewData["SupplierId"] = new SelectList(repositorySupplier.GetAll(), "ID", "Name");
        }


        // GET: Products/Edit/5
        [HttpGet]

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int Id)
        {
            var product = repository.GetProduct(Id);
            EditProductViewModel editProductViewModel = new EditProductViewModel();

            editProductViewModel.ID = product.ID;
            editProductViewModel.Name = product.Name;
            editProductViewModel.Image = product.Img;
            editProductViewModel.Price = product.Price;
            editProductViewModel.Detail = product.Detail;
            editProductViewModel.Unit = product.Unit;
            editProductViewModel.Discount = product.Discount;
            editProductViewModel.Stock = product.Stock;
            editProductViewModel.CategoryID = product.CategoryId;
            editProductViewModel.SupplierId = product.SupplierId;
            ViewData["CategoryID"] = new SelectList(repositorycaty.GetAll(), "ID", "Name", repositorycaty.GetCategory(editProductViewModel.CategoryID));
            ViewData["SupplierId"] = new SelectList(repositorySupplier.GetAll(), "ID", "Name", repositorySupplier.GetSupplier(editProductViewModel.SupplierId));

            return View(editProductViewModel);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(EditProductViewModel editProductViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string productNameToLower = editProductViewModel.Name.ToLower(); // Convert input name to lowercase

                    // Check if a product with the same name already exists for the same supplier and category
                    if (!repository.IsProductUnique(editProductViewModel.ID, productNameToLower, editProductViewModel.SupplierId, editProductViewModel.CategoryID))
                    {
                        ModelState.AddModelError("Name", "Product name already exists for the same supplier and category.");
                        PopulateDropdowns(editProductViewModel); // Populate dropdowns again for the view
                        return View(editProductViewModel);
                    }

                    repository.Update(editProductViewModel);
                    repository.Save();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    PopulateDropdowns(editProductViewModel); // Populate dropdowns again for the view
                    return View(editProductViewModel);
                }
            }
            else
            {
                PopulateDropdowns(editProductViewModel); // Populate dropdowns again for the view
                return View(editProductViewModel);
            }
        }

        [Authorize(Roles = "Admin")]
        private void PopulateDropdowns(EditProductViewModel editProductViewModel)
        {
            ViewData["CategoryID"] = new SelectList(repositorycaty.GetAll(), "ID", "Name", editProductViewModel.CategoryID);
            ViewData["SupplierId"] = new SelectList(repositorySupplier.GetAll(), "ID", "Name", editProductViewModel.SupplierId);
        }


        [Authorize(Roles = "Admin")]
        public IActionResult CheckProductExist(string Name)
        {
            if (repository.CheckProductExist(Name))
                return Json(true);
            else
                return Json(false);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CheckProductExistEdit(string Name, int Id)
        {
            if (repository.CheckProductExistEdit(Name, Id))
                return Json(true);
            else
                return Json(false);
        }

        // GET: Products/Delete/5
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int Id)
        {
            return View(repository.GetProduct(Id));
        }

        // POST: Products/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]

        [Authorize(Roles = "Admin")]
        public IActionResult ConfirmDelete(int Id)
        {
            var orderPending = context.Orders.Any(o => o.State == "Pending" && o.OrderProducts.Any(op => op.ProductId == Id));

            if (!orderPending)
            {
                if (ModelState.IsValid)
                {
                    repository.Delete(Id);
                    repository.Save();
                    return RedirectToAction("Index");
                }
                return View("Delete", repository.GetProduct(Id));
            }
            else
            {
                // Product has associated order products or is in pending orders, so return a view indicating it cannot be deleted.
                return View("cantDelete", repository.GetProduct(Id));
            }
        }


        [HttpGet]

        [Authorize(Roles = "Admin")]
        public IActionResult GetProduct(int Id)
        {
            var allProducts = repository.GetAll();
            var currentProduct = allProducts.FirstOrDefault(p => p.ID == Id);


            var randomProducts = allProducts.Except(new List<Product> { currentProduct }).OrderBy(x => Guid.NewGuid()).Take(4);

            ViewData["Products"] = randomProducts.ToList();
            var product = repository.GetProduct(Id);
            return View(product);
        }

    }
}
