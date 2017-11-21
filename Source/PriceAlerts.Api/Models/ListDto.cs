using System.Collections.Generic;

namespace PriceAlerts.Api.Models
{
    public class ListDto
    {
        public ListDto()
        {
            this.Alerts = new List<UserAlertSummaryDto>();
        }
        
        public string Id { get; set; }
        
        public string Name { get; set; }

        public IEnumerable<UserAlertSummaryDto> Alerts { get; set; }

        public override string ToString()
        {
            return $"{this.Name}";
        }
    }
}