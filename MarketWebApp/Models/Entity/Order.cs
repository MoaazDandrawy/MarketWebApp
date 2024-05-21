using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketWebApp.Models.Entity
{
    public class Order
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "State is required")]
        public string State { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; }

        // Navigation property to the join entity
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }

        [ForeignKey("Location")]
        public int LocationId { get; set; }
        public virtual Location Location { get; set; }

        [ForeignKey("applicationUser")]
        public string ApplicationUserID { get; set; }
        public virtual ApplicationUser applicationUser { get; set; }

     
    }
}
