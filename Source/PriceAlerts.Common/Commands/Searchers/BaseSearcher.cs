using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Searchers
{
    public abstract class BaseSearcher : ISearcher, IDisposable
    {
        private readonly IRequestClient _requestClient;

        protected BaseSearcher(IRequestClient requestClient, ISource source)
        {
            this._requestClient = requestClient;
            this._requestClient?.Initialize();

            this.Source = source;
        }

        protected ISource Source { get; }

        public void Dispose()
        {
            this._requestClient.Dispose();
        }

        [LoggingDescription("Searching HTML for URLs")]
        public async Task<IEnumerable<Uri>> GetProductsUrls(string searchTerm, int maxResultCount = 5)
        {
            var searchUrl = this.CreateSearchUri(searchTerm);

            // var document = await this.LoadDocument(searchUrl);

            var data = await this._requestClient.LoadHtmlAsync(searchUrl, this.Source.CustomHeaders.ToArray());

            if (data.IsSuccessStatusCode)
            {
                var content = await data.Content.ReadAsStringAsync();

                var document = new HtmlDocument();
                document.LoadHtml(System.Net.WebUtility.HtmlDecode(content));

                return this.GetSearchResultsUris(document, maxResultCount);
            }

            if (data.Headers.Location != null)
            {
                Uri location;
                if (Uri.IsWellFormedUriString(data.Headers.Location.ToString(), UriKind.Absolute))
                {
                    location = data.Headers.Location;
                }
                else
                {
                    location = new Uri(this.Source.Domain, data.Headers.Location);
                }

                Console.WriteLine("Redirect: " + location.AbsoluteUri);

                return new [] { location };
            }

            return Enumerable.Empty<Uri>();
        }

        protected abstract Uri CreateSearchUri(string searchTerm);

        protected abstract IEnumerable<Uri> GetSearchResultsUris(HtmlDocument doc, int maxResultCount);
    }
}
