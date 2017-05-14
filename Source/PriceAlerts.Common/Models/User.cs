using System.Collections.Generic;

namespace PriceAlerts.Common.Models
{
    public class User : Document
    {
        public User()
        {
            this.Settings = new Settings();
            this.Alerts = new List<UserAlert>();
        }

        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public Settings Settings { get; set; }

        public IList<UserAlert> Alerts { get; set; }
    }
}