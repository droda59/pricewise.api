using System;
using MongoDB.Bson.Serialization.Attributes;

namespace PriceAlerts.Common.Models
{
    [BsonIgnoreExtraElements]
    public class Deal
    {
        public decimal Price { get; set; }

        public string ProductId { get; set; }

        public DateTime ModifiedAt { get; set; }
    }
}