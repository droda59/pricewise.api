using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Autofac;

using PriceAlerts.Api.UrlCleaners;
using PriceAlerts.Common.Parsers;
using PriceAlerts.Common.Parsers.Models;
using PriceAlerts.Common.Searchers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Api.SourceHandlers
{
    internal abstract class BaseHandler : IHandler
    {
        private readonly ISource _source;

        public BaseHandler(ISource source, ICleaner cleaner, IParser parser, ISearcher searcher)
        {
            this._source = source;

            this.Cleaner = cleaner;
            this.Parser = parser;
            this.Searcher = searcher;
        }

        Uri IHandler.Domain => this._source.Domain;

        protected ICleaner Cleaner { get; }

        protected IParser Parser { get; }

        protected ISearcher Searcher { get; }

        public virtual Uri HandleCleanUrl(Uri url)
        {
            return this.Cleaner.CleanUrl(url);
        }

        public virtual async Task<SitePriceInfo> HandleParse(Uri url)
        {
            var cleanUrl = this.HandleCleanUrl(url);
            
            var siteInfo = await this.Parser.GetSiteInfo(cleanUrl);
            siteInfo.Uri = cleanUrl.AbsoluteUri;

            return siteInfo;
        }

        public virtual async Task<IEnumerable<Uri>> HandleSearch(string searchTerm)
        {
            var productUrls = new List<Uri>();
            var searchResults = await this.Searcher.GetProductsUrls(searchTerm);
            foreach (var searchResult in searchResults)
            {
                productUrls.Add(this.HandleCleanUrl(searchResult));
            }

            return productUrls;
        }
    }
}