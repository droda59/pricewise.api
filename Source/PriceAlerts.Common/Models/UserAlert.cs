using System.Collections.Generic;

namespace PriceAlerts.Common.Models
{
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

        public PriceChange BestCurrentDeal { get; set; }

        public IList<UserAlertEntry> Entries { get; set; }
    }
}