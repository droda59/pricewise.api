namespace PriceAlerts.Common.Models
{
    public class SitePriceInfo
    {
        public string ProductIdentifier { get; set; }

        public string Uri { get; set; }

        public string Title { get; set; }

        public string ImageUrl { get; set; }

        public decimal Price { get; set; }

        public override string ToString()
        {
            var title = this.Title;
            if (title.Length > 30)
            {
                title = title.Trim().Substring(0, 30) + "...";
            }
            
            return $"{this.Uri}: {title} at ${this.Price}";
        }
    }
}