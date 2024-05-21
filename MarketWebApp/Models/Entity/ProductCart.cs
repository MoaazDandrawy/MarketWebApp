using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarketWebApp.Models.Entity
{
    public class ProductCart
    {
        [Key]
        public int ID { get; set; }


        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }
        [ForeignKey("ShoppingCart")]

        public int ShoppingCartID { get; set; }
        public virtual ShoppingCart ShoppingCart { get; set; }

        public int Quantity { get; set; }


    }
}
