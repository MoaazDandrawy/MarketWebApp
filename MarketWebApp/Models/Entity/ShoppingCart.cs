using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketWebApp.Models.Entity
{
    public class ShoppingCart
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("applicationUser")]
        public string ApplicationUserID { get; set; }
        public virtual ApplicationUser applicationUser { get; set; }
        public virtual ICollection<ProductCart> ProductCarts { get; set; }
    }

}

