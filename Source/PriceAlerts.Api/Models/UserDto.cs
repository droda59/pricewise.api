namespace PriceAlerts.Api.Models
{
    public class UserDto
    {
        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public Common.Models.Settings Settings { get; set; }
    }
}