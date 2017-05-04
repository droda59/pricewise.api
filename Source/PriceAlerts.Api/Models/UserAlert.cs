using System;
using System.Collections.Generic;

namespace PriceAlerts.Api.Models
{
    public class UserAlert
    {
        public UserAlert()
        {
            this.Entries = new List<UserAlertEntry>();
        }

        public string Id { get; set; }

        public string Title { get; set; }

        public string ImageUrl { get; set; }

        public bool IsActive { get; set; }

        public DateTime LastModifiedAt { get; set; }

        public Deal BestCurrentDeal { get; set; }

        public IList<UserAlertEntry> Entries { get; set; }
    }
}