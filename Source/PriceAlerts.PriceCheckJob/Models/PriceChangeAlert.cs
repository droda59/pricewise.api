using System;

namespace PriceAlerts.PriceCheckJob.Models
{
    public class PriceChangeAlert
    {
        public string FirstName { get; set; }

        public string EmailAddress { get; set; }

        public string AlertTitle { get; set; }

        public decimal NewPrice { get; set; }

        public decimal PreviousPrice { get; set; }

        public Uri ProductUri { get; set; }

        public Uri ImageUrl { get; set; }
    }
}