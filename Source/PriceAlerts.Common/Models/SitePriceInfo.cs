namespace PriceAlerts.Common.Models
{
    public class SitePriceInfo
    {
        public string ProductIdentifier { get; set; }

        public string Uri { get; set; }

        public string Title { get; set; }

        public string ImageUrl { get; set; }

        public decimal Price { get; set; }
    }
}