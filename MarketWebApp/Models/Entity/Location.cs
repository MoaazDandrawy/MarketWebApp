using System.ComponentModel.DataAnnotations;

namespace MarketWebApp.Models.Entity
{
    public class Location
    {
        [Key] 
        public int ID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name must be between 1 and 100 characters", MinimumLength = 1)]
        public string Name { get; set; }

        [Display(Name = "Orders")] // Display name for the Orders property
        public virtual ICollection<Order> Orders { get; set; }

    }
}
