using System;
using System.Text.RegularExpressions;

using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Cleaners.Sources
{
    public class ShopifyCleaner : BaseCleaner, ICleaner
    {
        private readonly Regex _productPageExpression;

        public ShopifyCleaner()
        {
            this._productPageExpression = new Regex("products/.*", RegexOptions.Compiled);
        }

        public override Uri CleanUrl(Uri originalUrl)
        {
            var urlWithoutQueryString = new UriBuilder(originalUrl) { Query = string.Empty }.Uri;
            
            var productUrl = this._productPageExpression.Match(urlWithoutQueryString.ToString()).Value;
            
            var domain = originalUrl.Authority;

            var cleanedUrl = $"{originalUrl.Scheme}://{domain}/{productUrl}/"; // The trailing slash (/) must not be removed
            
            return new Uri(cleanedUrl); 
        }
    }
}