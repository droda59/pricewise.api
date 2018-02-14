using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace PriceAlerts.Common.Models
{
    [BsonIgnoreExtraElements]
    public class PriceCheckRunStatistics : Document
    {
        public PriceCheckRunStatistics()
        {
            this.Results = new List<PriceCheckRunDomainStatistics>();
        }
        
        public DateTime RunAt { get; set; }

        public IEnumerable<PriceCheckRunDomainStatistics> Results { get; set; }
    }

    public class PriceCheckRunDomainStatistics
    {
        public string Domain { get; set; }

        public int Errors { get; set; }
        
        public int Successes { get; set; }

        public int Unhandled { get; set; }
    }
}