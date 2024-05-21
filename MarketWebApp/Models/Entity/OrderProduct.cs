using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketWebApp.Models.Entity
{
    public class OrderProduct
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        [ForeignKey("Product")]

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public int Quantity { get; set; }
        public float Price { get; set; }
    }
}
