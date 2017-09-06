using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Autofac;

using PriceAlerts.Api.LinkManipulators;
using PriceAlerts.Api.LinkManipulators.UrlCleaners;
using PriceAlerts.Common.Parsers;
using PriceAlerts.Common.Parsers.Models;
using PriceAlerts.Common.Searchers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Api.SourceHandlers
{
    internal abstract class BaseHandler : IHandler
    {
        private readonly ISource _source;

        protected BaseHandler(ISource source, ICleaner manipulator, IParser parser, ISearcher searcher)
        {
            this._source = source;

            this.Parser = parser;
            this.Searcher = searcher;
            this.Manipulator = manipulator;
        }

        Uri IHandler.Domain => this._source.Domain;

        protected IParser Parser { get; }

        protected ISearcher Searcher { get; }

        protected ICleaner Manipulator { get; }

        public virtual Uri HandleCleanUrl(Uri url)
        {
            return this.Manipulator.CleanUrl(url);
        }

        public virtual Uri HandleManipulateUrl(Uri url)
        {
            var manipulator = this.Manipulator as ILinkManipulator;
            if (manipulator != null)
            {
                return manipulator.ManipulateLink(url);
            }

            return url;
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