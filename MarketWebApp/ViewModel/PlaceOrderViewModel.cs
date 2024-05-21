using MarketWebApp.Models.Entity;

namespace MarketWebApp
{
    public class PlaceOrderViewModel
    {
        public ShoppingCart ShoppingCart { get; set; }
        public List<Location> Locations { get; set; }
        public int SelectedLocationId { get; set; }
    }
}
