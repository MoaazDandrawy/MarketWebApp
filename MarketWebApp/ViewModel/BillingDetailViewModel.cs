namespace MarketWebApp.ViewModel
{
    public class BillingDetailViewModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }

        public float UnitPrice { get; set; }
        public float Discount { get; set; }
        public float TotalWithDiscount { get; set; }
        public float Total { get; set; }
    }
}
