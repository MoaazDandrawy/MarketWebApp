using static MarketWebApp.Controllers.BillingController;

namespace MarketWebApp.ViewModel
{
    public class BillingViewModel
    {
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public List<BillingDetailViewModel> BillingDetails { get; set; }
        public float TotalAmount { get; set; }
    }
}
