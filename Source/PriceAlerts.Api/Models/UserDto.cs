using System.Collections.Generic;

namespace PriceAlerts.Api.Models
{
    public class UserDto
    {
        public UserDto()
        {
            this.Alerts = new List<UserAlertDto>();
        }

        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public Common.Models.Settings Settings { get; set; }

        public IList<UserAlertDto> Alerts { get; set; }
    }
}