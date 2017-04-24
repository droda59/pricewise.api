using System;
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

        protected BaseParser(IHtmlLoader htmlLoader, Uri baseUri)
        {
            this._htmlLoader = htmlLoader;
            this._baseUri = baseUri;

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
            var document = await this.LoadDocument(uri);

            var sitePriceInfo = new SitePriceInfo
            {
                Uri = uri.AbsoluteUri,
                Title = this.GetTitle(document),
                ImageUrl = this.GetImageUrl(document),
                Price = this.GetPrice(document)
            };

            return sitePriceInfo;
        }

        protected abstract string GetTitle(HtmlDocument doc);

        protected abstract string GetImageUrl(HtmlDocument doc);

        protected abstract decimal GetPrice(HtmlDocument doc);

        protected string ExtractNumber(string original)
        {
            return new string(original.Where(c => Char.IsDigit(c) || Char.IsPunctuation(c)).ToArray());
        }

        private async Task<HtmlDocument> LoadDocument(Uri uri)
        {
            var content = await this._htmlLoader.ReadHtmlAsync(uri);
            
            var document = new HtmlDocument();
            
            document.LoadHtml(System.Net.WebUtility.HtmlDecode(content));

            return document;
        }
    }
}
