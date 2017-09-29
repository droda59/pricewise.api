using MongoDB.Bson.Serialization.Attributes;

namespace PriceAlerts.Common.Models
{
    [BsonIgnoreExtraElements]
    public class Settings
    {
        public Settings()
        {
            this.AlertOnPriceDrop = true;
            this.AlertOnPriceRaise = false;
            this.SpecifyChangePercentage = false;
            this.ChangePercentage = 0.1m;
        }

        public bool AlertOnPriceDrop { get; set; }

        public bool AlertOnPriceRaise { get; set; }

        public bool SpecifyChangePercentage { get; set; }

        public decimal ChangePercentage { get; set; }
    }
}