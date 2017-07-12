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

        public async Task<IEnumerable<Uri>> GetProductsUrls(string searchTerm)
        {
            return await Task.FromResult(Enumerable.Empty<Uri>());
        }

        protected void AddCustomHeaders(string header, string value)
        {
            this._customHeaders.Add(new KeyValuePair<string, string>(header, value));
        }

        protected async Task<HtmlDocument> LoadDocument(Uri uri)
        {
            var content = await this._htmlLoader.ReadHtmlAsync(uri, this._customHeaders.ToArray());

            var document = new HtmlDocument();
            
            document.LoadHtml(System.Net.WebUtility.HtmlDecode(content));

            return document;
        }
    }
}
