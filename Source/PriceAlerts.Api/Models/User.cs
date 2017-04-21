using System.Collections.Generic;

namespace PriceAlerts.Api.Models
{
    public class User
    {
        public User()
        {
            this.Alerts = new List<UserAlert>();
        }

        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public IList<UserAlert> Alerts { get; set; }
    }
}