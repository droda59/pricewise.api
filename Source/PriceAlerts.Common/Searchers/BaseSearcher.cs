using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HtmlAgilityPack;

using PriceAlerts.Common.Parsers.Models;

namespace PriceAlerts.Common.Parsers
{
    internal abstract class BaseSearcher : IDisposable
    {
        private readonly IHtmlLoader _htmlLoader;
        private readonly Uri _baseUri;
        private readonly IList<KeyValuePair<string, string>> _customHeaders;

        protected BaseSearcher(IHtmlLoader htmlLoader, Uri baseUri)
        {
            this._htmlLoader = htmlLoader;
            this._baseUri = baseUri;
            this._customHeaders = new List<KeyValuePair<string, string>>();

            if (this._htmlLoader != null)
            {
                this._htmlLoader.Initialize();
            }
        }

        public Uri Domain => this._baseUri;

        public void Dispose()
        {
            this._htmlLoader.Dispose();
        }

        public async Task<IEnumerable<Uri>> GetProductsUrls(string searchTerm, int maxResultCount = 5)
        {
            var searchUrl = this.CreateSearchUri(searchTerm);

            // var document = await this.LoadDocument(searchUrl);

            var data = await this._htmlLoader.LoadHtmlAsync(searchUrl, this._customHeaders.ToArray());
            
            if (data.IsSuccessStatusCode)
            {
                var content = await data.Content.ReadAsStringAsync();

                var document = new HtmlDocument();
                document.LoadHtml(System.Net.WebUtility.HtmlDecode(content));

                return this.GetSearchResultsUris(document, maxResultCount);
            }
            else if (data.Headers.Location != null)
            {
                Uri location;
                if (Uri.IsWellFormedUriString(data.Headers.Location.ToString(), UriKind.Absolute))
                {
                    location = data.Headers.Location;
                }
                else
                {
                    location = new Uri(this.Domain, data.Headers.Location);
                }

                Console.WriteLine("Redirect: " + location.AbsoluteUri);

                return new [] { location };
            }

            return Enumerable.Empty<Uri>();
        }

        protected void AddCustomHeaders(string header, string value)
        {
            this._customHeaders.Add(new KeyValuePair<string, string>(header, value));
        }

        protected async Task<HtmlDocument> LoadDocument(Uri uri)
        {
            HtmlDocument document = null;
            var data = await this._htmlLoader.LoadHtmlAsync(uri, this._customHeaders.ToArray());
            
            if (data.IsSuccessStatusCode)
            {
                var content = await data.Content.ReadAsStringAsync();

                document = new HtmlDocument();
                document.LoadHtml(System.Net.WebUtility.HtmlDecode(content));
            }

            return document;
        }

        protected abstract Uri CreateSearchUri(string searchTerm);

        protected abstract IEnumerable<Uri> GetSearchResultsUris(HtmlDocument doc, int maxResultCount);
    }
}
