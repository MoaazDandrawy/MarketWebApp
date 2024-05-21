using MarketWebApp.Models.Entity;

namespace MarketWebApp.ViewModel
{
    public class UserOrdersViewModel
    {
        public ApplicationUser User { get; set; }
        public List<Models.Entity.Order> Orders { get; set; }
    }
}
