using System.ComponentModel.DataAnnotations;

namespace MarketWebApp.Models.Entity
{
    public class Supplier
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        public string Location { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
