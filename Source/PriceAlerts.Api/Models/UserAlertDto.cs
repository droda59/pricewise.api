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

        public string Title { get; set; }

        public string ImageUrl { get; set; }

        public bool IsActive { get; set; }

        public DealDto BestCurrentDeal { get; set; }

        public IList<UserAlertEntryDto> Entries { get; set; }
    }
}