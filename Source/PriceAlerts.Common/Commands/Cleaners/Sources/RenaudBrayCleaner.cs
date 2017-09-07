using System;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Cleaners.Sources
{
    internal class RenaudBrayCleaner : ICleaner
    {
        private readonly RenaudBraySource _source;

        public RenaudBrayCleaner(RenaudBraySource source)
        {
            _source = source;
        }

        public Uri CleanUrl(Uri originalUrl)
        {
            StringValues id;
            var queryParameters = QueryHelpers.ParseQuery(originalUrl.Query);
            if (queryParameters.TryGetValue("id", out id) && this._source.IdExpression.IsMatch(id))
            {
                var urlWithoutQueryString = new UriBuilder(originalUrl) { Query = string.Empty };
                var urlWithQueryString = QueryHelpers.AddQueryString(urlWithoutQueryString.Uri.AbsoluteUri, "id", id);

                return new Uri(urlWithQueryString);
            }

            return null;
        }
    }
}