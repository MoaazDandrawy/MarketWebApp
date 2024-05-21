using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketWebApp.Models.Entity
{
    public class Product
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Price is required")]
        public float Price { get; set; }

        public float Discount { get; set; }
        [Display(Name = "Image")]
        public string Img { get; set; }
        public string Detail { get; set; }
        public string Unit { get; set; }

        [Required(ErrorMessage = "Stock is required")]
        public int Stock { get; set; }

        [ForeignKey("Supplier")]
        public int SupplierId { get; set; }
        public virtual Supplier Supplier { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        // Navigation property to the join entity
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
        public virtual ICollection<ProductCart> ProductCarts { get; set; }
       


    }
}
