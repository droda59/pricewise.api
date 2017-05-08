using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        protected void AddCustomHeaders(string header, string value)
        {
            this._customHeaders.Add(new KeyValuePair<string, string>(header, value));
        }

        protected string ExtractNumber(string original)
        {
            var cleanString = new string(original.Where(c => Char.IsDigit(c) || Char.IsPunctuation(c)).ToArray());
            if (cleanString.Contains(","))
            {
                var numberBuilder = new StringBuilder();
                var splitOnComma = cleanString.Split(',');
                for (var i = 0; i < splitOnComma.Length; i++)
                {
                    var part = splitOnComma[i];

                    if (i == splitOnComma.Length - 1 && !splitOnComma[i].Contains("."))
                    {
                        numberBuilder.Append(".");
                    }

                    numberBuilder.Append(part);
                }
            } 

            return cleanString;
        }

        private async Task<HtmlDocument> LoadDocument(Uri uri)
        {
            var content = await this._htmlLoader.ReadHtmlAsync(uri, this._customHeaders.ToArray());
            
            var document = new HtmlDocument();
            
            document.LoadHtml(System.Net.WebUtility.HtmlDecode(content));

            return document;
        }
    }
}
