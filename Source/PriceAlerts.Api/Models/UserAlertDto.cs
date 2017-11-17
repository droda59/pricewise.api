using System;
using System.Collections.Generic;

namespace PriceAlerts.Api.Models
{
    public class UserAlertDto
    {
        public UserAlertDto()
        {
            this.Entries = new List<UserAlertEntryDto>();
        }

        public string Id { get; set; }

        public DateTime ModifiedAt { get; set; }

        public IList<UserAlertEntryDto> Entries { get; set; }

        public override string ToString()
        {
            return $"{this.Id}";
        }
    }
}