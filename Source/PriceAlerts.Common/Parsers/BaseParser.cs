using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HtmlAgilityPack;

using PriceAlerts.Common.Parsers.Models;

namespace PriceAlerts.Common.Parsers
{
    internal abstract class BaseParser : IDisposable
    {
        private readonly IHtmlLoader _htmlLoader;
        private readonly Uri _baseUri;
        private readonly IList<KeyValuePair<string, string>> _customHeaders;

        protected BaseParser(IHtmlLoader htmlLoader, Uri baseUri)
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

        public async Task<SitePriceInfo> GetSiteInfo(string uri)
        {
            return await this.GetSiteInfo(new Uri(uri));
        }

        public async Task<SitePriceInfo> GetSiteInfo(Uri uri)
        {
            string title;
            string imageUrl;
            decimal price;
            Uri productUrl = uri;

            var document = await this.LoadDocument(productUrl);

            if (this.HasRedirectProductUrl(document))
            {
                productUrl = this.GetRedirectProductUrl(document);
                document = await this.LoadDocument(productUrl);
            }

            try
            {
                title = this.GetTitle(document);
            }
            catch (Exception e)
            {
                throw new ParseException("Error parsing the title", e, uri);
            }
            
            try
            {
                imageUrl = this.GetImageUrl(document);
            }
            catch (Exception e)
            {
                throw new ParseException("Error parsing the image", e, uri);
            }
            
            try
            {
                price = this.GetPrice(document);
            }
            catch (Exception e)
            {
                throw new ParseException("Error parsing the price", e, uri);
            }

            var sitePriceInfo = new SitePriceInfo
            {
                Uri = productUrl.AbsoluteUri,
                Title = title,
                ImageUrl = imageUrl,
                Price = price
            };

            return sitePriceInfo;
        }

        protected abstract string GetTitle(HtmlDocument doc);

        protected abstract string GetImageUrl(HtmlDocument doc);

        protected abstract decimal GetPrice(HtmlDocument doc);

        protected virtual Uri GetRedirectProductUrl(HtmlDocument doc)
        {
            return null;
        }

        protected virtual bool HasRedirectProductUrl(HtmlDocument doc)
        {
            return false;
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
