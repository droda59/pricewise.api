using MongoDB.Bson.Serialization.Attributes;

namespace PriceAlerts.Common.Models
{
    [BsonIgnoreExtraElements]
    public class UserAlertEntry
    {
        public string MonitoredProductId { get; set; }

        public bool IsDeleted { get; set; }
    }
}