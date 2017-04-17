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

        public DateTime LastUpdate { get; set; }

        public decimal BestCurrentPrice { get; set; }

        public IList<UserAlertEntry> Entries { get; set; }
    }
}