using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MarketWebApp.ViewModel.Order
{
    public class EditOrderViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "State is required")]
        public string State { get; set; }

        [Required(ErrorMessage = "Order request date can't be empty")]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        [Display(Name = "Order Location ")]
        public int LocationID { get; set; }

        [Display(Name = "Order Cost")]
        [Range(1, 10000, ErrorMessage = "Order Cost Must Be Between 1 and 10000")]
        public float Cost { get; set; }

        public string applicationUserId { get; set; }
    }
}
