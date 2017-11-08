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

        public DateTime ModifiedAt { get; set; }

        public IList<UserAlertEntryDto> Entries { get; set; }

        public override string ToString()
        {
            var title = this.Title.Length > 30 
                ? this.Title.Trim().Substring(0, 30) + "..." 
                : this.Title;
            
            return $"{this.Id}: {title}";
        }
    }
}