using MarketWebApp.Repository.SupplierRepository;
using MarketWebApp.ViewModel.Supplier;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;

namespace MarketWebApp.Controllers
{

    [Authorize(Roles = "Admin")]
    public class SuppliersController : Controller
    {

        private readonly ISupplierRepository supplierRepository;
        // GET: SupplierController

        public SuppliersController(ISupplierRepository supplierRepository)
        {
            this.supplierRepository = supplierRepository;
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {

            ViewBag.PageCount = (int)Math.Ceiling((decimal)supplierRepository.GetAll().Count() / 5m);

            return View(supplierRepository.GetAll());
        }


        [Authorize(Roles = "Admin")]
        public IActionResult GetSupplier(int pageNumber, int pageSize = 5, string searchQuery = "")
        {
            // Check if there's a search query
            if (!string.IsNullOrEmpty(searchQuery))
            {
                var suppliers = supplierRepository.SearchByName(searchQuery)
                    .OrderBy(p => p.ID)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return PartialView("_SupplierTable", suppliers);
            }
            else
            {
                var suppliers = supplierRepository.GetAll()
                    .OrderBy(p => p.ID)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return PartialView("_SupplierTable", suppliers);
            }
        }
        // GET: SupplierController/Create

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: SupplierController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]

        [Authorize(Roles = "Admin")]
        public IActionResult Create(AddSupplierViewModel addSupplierViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string supplierNameToLower = addSupplierViewModel.Name.ToLower(); // Convert input name to lowercase

                    if (supplierRepository.GetAll().Any(s => s.Name.ToLower() == supplierNameToLower))
                    {
                        ModelState.AddModelError("", "Supplier with the same name already exists.");
                        return View(addSupplierViewModel);
                    }

                    supplierRepository.Insert(addSupplierViewModel);
                    supplierRepository.Save();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(addSupplierViewModel);
                }
            }
            else
            {
                return View(addSupplierViewModel);
            }
        }


        [Authorize(Roles = "Admin")]
        public IActionResult CheckSupplierExist(string Name)
        {
            if (supplierRepository.CheckSupplierExist(Name))
                return Json(true);
            else
                return Json(false);
        }

        // GET: SupplierController/Edit/5

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Edit(int Id)
        {
            var supplier = supplierRepository.GetSupplier(Id);
            EditSupplierViewModel supplierViewModel = new EditSupplierViewModel();
            supplierViewModel.ID = supplier.ID;
            supplierViewModel.Name = supplier.Name;
            supplierViewModel.Phone = supplier.Phone;
            supplierViewModel.Location = supplier.Location;
            return View(supplierViewModel);
        }

        // POST: SupplierController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(EditSupplierViewModel supplierViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string supplierNameToLower = supplierViewModel.Name.ToLower(); // Convert input name to lowercase

                    // Check if supplier with the same name (case-insensitive) already exists, excluding the current supplier being edited
                    if (supplierRepository.GetAll().Any(s => s.Name.ToLower() == supplierNameToLower && s.ID != supplierViewModel.ID))
                    {
                        ModelState.AddModelError("Name", "Supplier name already exists.");
                        return View(supplierViewModel);
                    }

                    supplierRepository.Update(supplierViewModel);
                    supplierRepository.Save();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(supplierViewModel);
                }
            }
            else
            {
                return View(supplierViewModel);
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CheckSupplierExistEdit(string Name, int Id)
        {
            if (supplierRepository.CheckSupplierExistEdit(Name, Id))
                return Json(true);
            else
                return Json(false);
        }
        // GET: SupplierController/Delete/5

        [HttpGet]

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int Id)
        {
            var data = supplierRepository.GetSupplierWithProducts(Id);
            ViewBag.flag = data.Products.Count > 0 ? true : false;
            return View(data);
        }

        // POST: SupplierController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]

        [Authorize(Roles = "Admin")]
        public IActionResult ConfirmDelete(int Id)
        {

            if (ModelState.IsValid)
            {
                supplierRepository.Delete(Id);
                supplierRepository.Save();
                return RedirectToAction("Index");
            }
            return View("Delete");
        }


       
    }
}
