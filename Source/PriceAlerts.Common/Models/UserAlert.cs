using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace PriceAlerts.Common.Models
{
    [BsonIgnoreExtraElements]
    public class UserAlert : Document
    {
        public UserAlert()
        {
            this.Entries = new List<UserAlertEntry>();
        }

        public string Title { get; set; }

        public string ImageUrl { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime LastModifiedAt { get; set; }

        public Deal BestCurrentDeal { get; set; }

        public IList<UserAlertEntry> Entries { get; set; }

        public override bool Equals(object obj)
        {
            return this.Id == ((UserAlert)obj).Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public override string ToString()
        {
            var id = this.Id;
            if (string.IsNullOrEmpty(id))
            {
                return "New alert";
            }
            
            var title = this.Title;
            if (title.Length > 30)
            {
                title = title.Trim().Substring(0, 30) + "...";
            }
            
            return $"{id}: {title}";
        }
    }
}