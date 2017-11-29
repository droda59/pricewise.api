using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace PriceAlerts.Common.Models
{
    [BsonIgnoreExtraElements]
    public class List : Document
    {
        public List()
        {
            this.Alerts = new List<UserAlert>();
        }
        
        public string Name { get; set; }

        public string UserId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastModifiedAt { get; set; }

        // TODO Voir si je peux mettre internal
        public bool IsDeleted { get; set; }

        public bool IsPublic { get; set; }

        public IEnumerable<UserAlert> Alerts { get; set; }

        public override string ToString()
        {
            var id = this.Id;
            if (string.IsNullOrEmpty(id))
            {
                return "New list";
            }
            
            var name = this.Name;
            if (name.Length > 30)
            {
                name = name.Trim().Substring(0, 30) + "...";
            }
            
            return $"{id}: {name}";
        }
    }
}