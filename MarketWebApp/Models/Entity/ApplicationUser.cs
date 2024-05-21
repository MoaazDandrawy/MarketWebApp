using Microsoft.AspNetCore.Identity;

namespace MarketWebApp.Models.Entity
{
    public class ApplicationUser: IdentityUser
    {

        public string Address { get; set; }
        public virtual ICollection<Order> Orders { get; set; }

    }

}
