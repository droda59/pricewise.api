using System.Collections.Generic;

namespace PriceAlerts.Common.Models
{
    public class User : Document
    {
        public User()
        {
            this.Alerts = new List<UserAlert>();
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public IList<UserAlert> Alerts { get; set; }
    }
}