using System;
using MongoDB.Bson.Serialization.Attributes;

namespace PriceAlerts.Common.Models
{
    [BsonIgnoreExtraElements]
    public class PriceChange : IComparable<PriceChange>
    {
        public decimal Price { get; set; }

        public DateTime ModifiedAt { get; set; }

        int IComparable<PriceChange>.CompareTo(PriceChange other)
        {
            if (other.Price > this.Price)
            {
                return -1;
            }
            else if (other.Price == this.Price)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
    }
}