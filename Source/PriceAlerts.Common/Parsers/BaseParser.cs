using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using HtmlAgilityPack;

using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Parsers.Models;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Parsers
{
    public abstract class BaseParser
    {
        private readonly IDocumentLoader _documentLoader;

        protected BaseParser(IDocumentLoader documentLoader, ISource source)
        {
            this._documentLoader = documentLoader;

            this.Source = source;
        }
        
        public ISource Source { get; }

        public async Task<SitePriceInfo> GetSiteInfo(Uri url)
        {
            string productIdentifier;
            string title;
            string imageUrl;
            decimal price;
            Uri productUrl = url;

            var document = await this._documentLoader.LoadDocument(productUrl, this.Source.CustomHeaders);

            if (this.HasRedirectProductUrl(document))
            {
                productUrl = this.GetRedirectProductUrl(document);
                document = await this._documentLoader.LoadDocument(productUrl, this.Source.CustomHeaders);
            }

            try
            {
                title = this.GetTitle(document);
            }
            catch (Exception e)
            {
                throw new ParseException("Error parsing the title", e, productUrl);
            }
            
            try
            {
                imageUrl = this.GetImageUrl(document);
            }
            catch (Exception e)
            {
                throw new ParseException("Error parsing the image", e, productUrl);
            }
            
            try
            {
                price = this.GetPrice(document);
            }
            catch (Exception e)
            {
                throw new ParseException("Error parsing the price", e, productUrl);
            }

            try
            {
                productIdentifier = this.GetProductIdentifier(document);
            }
            catch (Exception e)
            {
                throw new ParseException("Error parsing the product identifier", e, productUrl);
            }

            var sitePriceInfo = new SitePriceInfo
            {
                ProductIdentifier = productIdentifier, 
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

        protected virtual string GetProductIdentifier(HtmlDocument doc)
        {
            return string.Empty;
        }

        protected virtual Uri GetRedirectProductUrl(HtmlDocument doc)
        {
            return null;
        }

        protected virtual bool HasRedirectProductUrl(HtmlDocument doc)
        {
            return false;
        }
    }
}
