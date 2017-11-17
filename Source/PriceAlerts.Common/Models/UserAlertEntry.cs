using System;
using MongoDB.Bson.Serialization.Attributes;

namespace PriceAlerts.Common.Models
{
    [BsonIgnoreExtraElements]
    public class UserAlertEntry
    {
        public string MonitoredProductId { get; set; }

        public string Note { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public decimal OriginalPrice { get; set; }
    }
}