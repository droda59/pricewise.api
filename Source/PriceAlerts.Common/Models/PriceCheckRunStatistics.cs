using System;
using System.Collections.Generic;
<<<<<<< HEAD
=======
using System.Globalization;
>>>>>>> master
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
<<<<<<< HEAD
=======

        public override string ToString()
        {
            return $"{this.RunAt.ToString(CultureInfo.CurrentCulture)}";
        }
>>>>>>> master
    }

    public class PriceCheckRunDomainStatistics
    {
        public string Domain { get; set; }

        public int Errors { get; set; }
        
        public int Successes { get; set; }

        public int Unhandled { get; set; }
    }
}