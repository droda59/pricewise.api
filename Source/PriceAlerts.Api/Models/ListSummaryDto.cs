namespace PriceAlerts.Api.Models
{
    public class ListSummaryDto
    {
        public string Id { get; set; }
        
        public string Name { get; set; }

        public override string ToString()
        {
            return $"{this.Name}";
        }
    }
}