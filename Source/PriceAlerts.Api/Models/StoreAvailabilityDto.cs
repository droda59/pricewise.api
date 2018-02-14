namespace PriceAlerts.Api.Models
{
    public class StoreAvailabilityDto
    {
        public string Domain { get; set; }

        public bool IsAvailable { get; set; }
        
        public override string ToString()
        {
            return $"{this.Domain}";
        }
    }
}