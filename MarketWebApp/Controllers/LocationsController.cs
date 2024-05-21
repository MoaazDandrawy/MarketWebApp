using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MarketWebApp.Data;
using MarketWebApp.Models.Entity;
using MarketWebApp.Repository.LocationRepository;
using MarketWebApp.Repository.SupplierRepository;
using MarketWebApp.ViewModel;
using MarketWebApp.ViewModel.Location;
using NuGet.Protocol.Core.Types;
using Microsoft.AspNetCore.Authorization;

namespace MarketWebApp.Controllers
{
    public class LocationsController : Controller
    {
        private readonly ILocationRepository locationRepository;

        public LocationsController(ILocationRepository locationRepository)
        {
            this.locationRepository = locationRepository;
        }

        // GET: Locations
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            ViewBag.PageCount = (int)Math.Ceiling((decimal)locationRepository.GetAll().Count() / 5m);

            return View(locationRepository.GetAll());
        }

        // GET: Locations/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult CheckLocationExist(string Name)
        {
            if (locationRepository.CheckLocationExist(Name))
                return Json(true);
            else
                return Json(false);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult CheckLocationExistEdit(string Name, int Id)
        {
            if (locationRepository.CheckLocationExistEdit(Name, Id))
                return Json(true);
            else
                return Json(false);
        }

        // POST: Locations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(AddLocationViewModel locationViewModel)
        {
            if (ModelState.IsValid)
            {
                string locationNameToLower = locationViewModel.Name.ToLower(); // Convert input name to lowercase

                if (locationRepository.GetAll().Any(c => c.Name.ToLower() == locationNameToLower))
                {
                    ModelState.AddModelError("", "Location with the same name already exists.");
                    return View(locationViewModel);
                }

                try
                {
                    locationRepository.Insert(locationViewModel);
                    locationRepository.Save();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(locationViewModel);
                }
            }
            else
            {
                return View(locationViewModel);
            }
        }


        // GET: Locations/Edit/5
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int Id)
        {
            var location = locationRepository.GetLocation(Id);
            EditLocationViewModel locationViewModel = new EditLocationViewModel();
            locationViewModel.ID = location.ID;
            locationViewModel.Name = location.Name;
            return View(locationViewModel);
        }

        // POST: Locations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(EditLocationViewModel locationViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string locationNameToLower = locationViewModel.Name.ToLower(); // Convert input name to lowercase

                    if (!locationRepository.IsLocationNameUnique(locationViewModel.ID, locationNameToLower))
                    {
                        ModelState.AddModelError("Name", "Location name already exists.");
                        return View(locationViewModel);
                    }

                    locationRepository.Update(locationViewModel);
                    locationRepository.Save();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    return View(locationViewModel);
                }
            }
            else
            {
                return View(locationViewModel);
            }
        }

        public IActionResult GetLocation(int pageNumber, int pageSize = 5, string searchQuery = "")
        {
            // Check if there's a search query
            if (!string.IsNullOrEmpty(searchQuery))
            {
                var locations = locationRepository.SearchByName(searchQuery)
                    .OrderBy(p => p.ID)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return PartialView("_LocationTable", locations);
            }
            else
            {
                var locations = locationRepository.GetAll()
                    .OrderBy(p => p.ID)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return PartialView("_LocationTable", locations);
            }
        }

        // GET: Locations/Delete/5
        [HttpGet]
        public IActionResult Delete(int Id)
        {
            return View(locationRepository.GetLocation(Id));
        }

        // POST: Locations/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmDelete(int Id)
        {
            if (ModelState.IsValid)
            {
                locationRepository.Delete(Id);
                locationRepository.Save();
                return RedirectToAction("Index");
            }
            return View("Delete");
        }
    }
}
