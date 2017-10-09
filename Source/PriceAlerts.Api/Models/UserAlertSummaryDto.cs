namespace PriceAlerts.Api.Models
{
    public class UserAlertSummaryDto
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string ImageUrl { get; set; }

        public bool IsActive { get; set; }

        public DealDto BestCurrentDeal { get; set; }

        public override string ToString()
        {
            var title = this.Title.Length > 30 
                ? this.Title.Trim().Substring(0, 30) + "..." 
                : this.Title;
            
            return $"{this.Id}: {title}";
        }
    }
}