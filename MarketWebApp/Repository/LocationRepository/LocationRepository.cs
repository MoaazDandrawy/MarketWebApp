using MarketWebApp.Data;
using MarketWebApp.Models.Entity;
using MarketWebApp.ViewModel.Location;
using Microsoft.EntityFrameworkCore;

namespace MarketWebApp.Repository.LocationRepository
{
    public class LocationRepository : ILocationRepository
    {
        private readonly ApplicationDbContext context;
        public LocationRepository(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
        }
        public bool CheckLocationExist(string Name)
        {
            return context.Locations.SingleOrDefault(b => b.Name.ToLower() == Name.ToLower()) == null;
        }
        public IEnumerable<Location> SearchByName(string name)
        {
            var locations = context.Locations
                .Where(s => s.Name.Contains(name))
                .ToList();

            if (locations == null || !locations.Any())
            {
                return Enumerable.Empty<Location>();
            }

            return locations;
        }
        public bool IsLocationNameUnique(int locationId, string locationName)
        {
            // Check if there is any other location with the same name but a different ID
            return !context.Locations.Any(c => c.ID != locationId && c.Name == locationName);
        }
        public bool CheckLocationExistEdit(string Name, int Id)
        {
            return context.Locations.SingleOrDefault(b => b.ID != Id && b.Name.ToLower() == Name.ToLower()) == null;
        }

        public void Delete(int id)
        {
            var location = GetLocation(id);

            if (location == null)
            {
                // Handle the case where the location with the given id is not found
                throw new InvalidOperationException("Location not found.");
            }

            context.Locations.Remove(location);
            Save();
        }

        public IEnumerable<Location> GetAll()
        {
            return context.Locations.ToList();
        }

        public Location GetLocation(int Id)
        {
            return context.Locations.SingleOrDefault(b => b.ID == Id);
        }

        //public Location GetLocationWithOrders(int Id)
        //{
        //    string strId = Id.ToString(); // Convert Id to string

        //    return context.Locations
        //        .Where(b => b.ID == Id)
        //        .Include(b => b.Orders.Where(o => o.ApplicationUserID == strId)) // Compare with string
        //        .SingleOrDefault();
        //}




        public void Insert(AddLocationViewModel addLocationViewModel)
        {
            var location = new Location();
            location.Name = addLocationViewModel.Name;
            context.Locations.Add(location);
            Save();
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Update(EditLocationViewModel editLocationViewModel)
        {
            var location = GetLocation(editLocationViewModel.ID);

            if (location == null)
            {
                throw new ArgumentException("Location not found.");
            }

            if (string.IsNullOrWhiteSpace(editLocationViewModel.Name))
            {
                throw new ArgumentException("Location name cannot be empty.");
            }

            location.Name = editLocationViewModel.Name;

            Save();
        }
    }
}
