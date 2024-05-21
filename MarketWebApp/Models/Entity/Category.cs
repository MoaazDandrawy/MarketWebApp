using System.ComponentModel.DataAnnotations;

namespace MarketWebApp.Models.Entity
{
    public class Category
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "Name is required")] 
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name length must be between 3 and 100 characters")] // Specifies the length constraints for Name
        public string ?Name { get; set; }
        public string? Img { get; set; }

        [Display(Name = "Products")] // Specifies the display name for Products
        public virtual ICollection<Product>? Products { get; set; }

    }
}
