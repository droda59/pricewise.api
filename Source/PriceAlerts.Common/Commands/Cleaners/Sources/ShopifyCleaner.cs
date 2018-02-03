using System;

using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Cleaners.Sources
{
    public class ShopifyCleaner : BaseCleaner, ICleaner
    {
        private readonly ShopifySource _source;

        public ShopifyCleaner(ShopifySource source)
        {
            this._source = source;
        }

        public override Uri CleanUrl(Uri originalUrl)
        {
            var urlWithoutQueryString = new UriBuilder(originalUrl) { Query = string.Empty }.Uri;
            
            var newUrl = this._source.ProductPageExpression.Match(urlWithoutQueryString.ToString()).Value;
            
            return new Uri(this._source.Domain, newUrl);
        }
    }
}