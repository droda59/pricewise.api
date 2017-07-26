using System;
using System.Linq;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace PriceAlerts.Common.Cleaners.Sources
{
    internal class RenaudBrayCleaner : BaseCleaner, ICleaner
    {
        private readonly Regex _idExpression;

        public RenaudBrayCleaner()
            : base(new Uri("http://www.renaud-bray.com/"))
        {
            this._idExpression = new Regex(@"[0-9]{7}$", RegexOptions.Compiled);
        }

        public override Uri CleanUrl(Uri originalUrl)
        {
            StringValues id = StringValues.Empty;
            var queryParameters = QueryHelpers.ParseQuery(originalUrl.Query);
            if (queryParameters.TryGetValue("id", out id) && this._idExpression.IsMatch(id))
            {
                var urlWithoutQueryString = new UriBuilder(originalUrl) { Query = string.Empty };
                var urlWithQueryString = QueryHelpers.AddQueryString(urlWithoutQueryString.Uri.AbsoluteUri, "id", id);

                return new Uri(urlWithQueryString);
            }

            return null;
        }
    }
}