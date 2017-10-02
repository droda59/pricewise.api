using System;

namespace PriceAlerts.Api.Models
{
    public class UserAlertSummaryDto
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string ImageUrl { get; set; }

        public bool IsActive { get; set; }

        public DealDto BestCurrentDeal { get; set; }
    }
}