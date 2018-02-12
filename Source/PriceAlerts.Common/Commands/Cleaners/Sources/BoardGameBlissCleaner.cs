using System;

using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Cleaners.Sources
{
    public class BoardGameBlissCleaner : BaseCleaner, ICleaner
    {
        private readonly BoardGameBlissSource _source;

        public BoardGameBlissCleaner(BoardGameBlissSource source)
        {
            this._source = source;
        }

        public override Uri CleanUrl(Uri originalUrl)
        {
            var urlWithoutQueryString = new UriBuilder(originalUrl) { Query = string.Empty }.Uri;
            
            var newUrl = this._source.ProductExpression.Match(urlWithoutQueryString.ToString()).Value;
            
            return new Uri(this._source.Domain, newUrl);
        }
    }
}