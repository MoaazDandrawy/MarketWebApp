
using System.ComponentModel.DataAnnotations;

namespace MarketWebApp.ViewModel.OrderProduct
{
    public class AddOrderProductViewModel
    {
        [Display(Name = "Order Product Quantity")]
        [Range(1, 1000, ErrorMessage = "Product Quantity Must Be Between 1 and 1000")]
        public int Quantity { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        
    }
}
