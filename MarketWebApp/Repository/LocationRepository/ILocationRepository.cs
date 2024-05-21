using MarketWebApp.Models.Entity;
using MarketWebApp.ViewModel.Location;
using MarketWebApp.ViewModel.Supplier;

namespace MarketWebApp.Repository.LocationRepository
{
    public interface ILocationRepository
    {
        IEnumerable<Location> GetAll();
        IEnumerable<Location> SearchByName(string name);

        Location GetLocation(int Id);
        //Location GetLocationWithOrders(int Id);
        void Insert(AddLocationViewModel addLocationViewModel);
        void Update(EditLocationViewModel editLocationViewModel);
        void Delete(int id);
        void Save();
        bool CheckLocationExist(string Name);
        bool CheckLocationExistEdit(string Name, int Id);
        bool IsLocationNameUnique(int locationId, string locationName);

    }
}
