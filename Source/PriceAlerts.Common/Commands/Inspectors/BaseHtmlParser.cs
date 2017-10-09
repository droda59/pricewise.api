using System;
using System.Threading.Tasks;
using HtmlAgilityPack;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Models;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors
{
    public abstract class BaseHtmlParser : IInspector
    {
        private readonly IDocumentLoader _documentLoader;

        protected BaseHtmlParser(IDocumentLoader documentLoader, ISource source)
        {
            this._documentLoader = documentLoader;

            this.Source = source;
        }

        protected ISource Source { get; }

        [LoggingDescription("Parsing HTML")]
        public virtual async Task<SitePriceInfo> GetSiteInfo(Uri url)
        {
            string productIdentifier;
            string title;
            string imageUrl;
            decimal price;
            var productUrl = url;

            var document = await this._documentLoader.LoadDocument(productUrl, this.Source.CustomHeaders);

            if (this.HasRedirectProductUrl(document))
            {
                productUrl = this.GetRedirectProductUrl(document);
                document = await this._documentLoader.LoadDocument(productUrl, this.Source.CustomHeaders);
            }

            if (document == null)
            {
                throw new ParseException("Error parsing the document", productUrl);                
            }

            try
            {
                title = this.GetTitle(document);
            }
            catch (Exception e)
            {
                throw new ParseException("Error parsing the title: " + e.Message, e, productUrl);
            }
            
            try
            {
                imageUrl = this.GetImageUrl(document);
            }
            catch (Exception e)
            {
                throw new ParseException("Error parsing the image: " + e.Message, e, productUrl);
            }
            
            try
            {
                price = this.GetPrice(document);
            }
            catch (Exception e)
            {
                throw new ParseException("Error parsing the price: " + e.Message, e, productUrl);
            }

            try
            {
                productIdentifier = this.GetProductIdentifier(document);
            }
            catch (Exception e)
            {
                throw new ParseException("Error parsing the product identifier: " + e.Message, e, productUrl);
            }

            return new SitePriceInfo
            {
                ProductIdentifier = productIdentifier, 
                Uri = productUrl.AbsoluteUri,
                Title = title,
                ImageUrl = imageUrl,
                Price = price
            };
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
