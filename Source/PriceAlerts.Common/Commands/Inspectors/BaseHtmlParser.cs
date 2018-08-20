using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HtmlAgilityPack;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Models;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Commands.Inspectors
{
    public abstract class BaseHtmlParser : BaseParser<HtmlParserContext>
    {
        private readonly IDocumentLoader _documentLoader;

        protected BaseHtmlParser(IDocumentLoader documentLoader, ISource source)
        {
            this._documentLoader = documentLoader;

            this.Source = source;

            this.ParserSteps = new List<Action>
            {
                this.ParseProductIdentifier,
                this.ParseTitle,
                this.ParseImageUrl,
                this.ParsePrice,
            };
        }

        protected ISource Source { get; }

        [LoggingDescription("Parsing HTML")]
        public override async Task<SitePriceInfo> Parse(Uri url)
        {
            await InitializeParserContext(url);

            this.ExecuteParserSteps();

            return this.Context.SitePriceInfo;
        }

        protected virtual void ParseProductIdentifier()
        {
            this.Context.SitePriceInfo.ProductIdentifier = string.Empty;
        }

        protected abstract void ParseTitle();

        protected abstract void ParseImageUrl();

        protected abstract void ParsePrice();


        protected virtual Uri GetRedirectProductUrl(HtmlDocument doc)
        {
            return null;
        }

        protected virtual bool HasRedirectProductUrl(HtmlDocument doc)
        {
            return false;
        }

        private void ExecuteParserSteps()
        {
            foreach (var parserStep in ParserSteps)
            {
                try
                {
                    parserStep();
                }
                catch (Exception e)
                {
                    throw new ParseException("Error during parsing: " + e.Message, e, this.Context.SourceUri);
                }
            }
        }

        private async Task InitializeParserContext(Uri url)
        {
            this.Context = new HtmlParserContext(url, await GetHtmlDocument(url));
            this.ParseProductIdentifier();
        }

        private async Task<HtmlDocument> GetHtmlDocument(Uri productUrl)
        {
            var document = await this._documentLoader.LoadDocument(productUrl, this.Source.CustomHeaders);

            if (this.HasRedirectProductUrl(document))
            {
                productUrl = this.GetRedirectProductUrl(document);
                document = await this._documentLoader.LoadDocument(productUrl, this.Source.CustomHeaders);
            }

            return document;
        }
    }
}
