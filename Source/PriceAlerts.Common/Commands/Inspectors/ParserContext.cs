using System;
using PriceAlerts.Common.Models;

namespace PriceAlerts.Common.Commands.Inspectors
{
    public class ParserContext
    {
        public ParserContext(Uri sourceUri)
        {
            this.SourceUri = sourceUri;
            this.ProductIdentifier = string.Empty;
            this.SitePriceInfo = new SitePriceInfo();
        }

        public Uri SourceUri { get; set; }

        public string ProductIdentifier { get; set; }

        public SitePriceInfo SitePriceInfo { get; set; }
    }
}