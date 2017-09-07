using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PriceAlerts.Common.LinkManipulators;
using PriceAlerts.Common.LinkManipulators.UrlCleaners;
using PriceAlerts.Common.Parsers;
using PriceAlerts.Common.Parsers.Models;
using PriceAlerts.Common.Searchers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.SourceHandlers
{
    internal abstract class BaseHandler : IHandler
    {
        private readonly ISource _source;
        private readonly ICleaner _manipulator;
        private readonly ISearcher _searcher;
        private readonly IParser[] _parsers;

        protected BaseHandler(ISource source, ICleaner manipulator, IParser parser, ISearcher searcher)
            : this(source, manipulator, new[] { parser }, searcher)
        {
        }

        protected BaseHandler(ISource source, ICleaner manipulator, IParser[] parsers, ISearcher searcher)
        {
            this._source = source;
            
            this._parsers = parsers;
            this._searcher = searcher;
            this._manipulator = manipulator;
        }

        Uri IHandler.Domain => this._source.Domain;

        public virtual Uri HandleCleanUrl(Uri url)
        {
            return this._manipulator.CleanUrl(url);
        }

        public virtual Uri HandleManipulateUrl(Uri url)
        {
            var manipulator = this._manipulator as ILinkManipulator;
            if (manipulator != null)
            {
                return manipulator.ManipulateLink(url);
            }

            return url;
        }

        public virtual async Task<SitePriceInfo> HandleParse(Uri url)
        {
            SitePriceInfo siteInfo = null;
            var cleanUrl = this.HandleCleanUrl(url);

            var currentIndex = 0;
            while (siteInfo == null && currentIndex < this._parsers.Length)
            {
                siteInfo = await this._parsers[currentIndex].GetSiteInfo(cleanUrl);
                currentIndex++;
            }

            if (siteInfo != null)
            {
                siteInfo.Uri = cleanUrl.AbsoluteUri;
            }

            return siteInfo;
        }

        public virtual async Task<IEnumerable<Uri>> HandleSearch(string searchTerm)
        {
            var productUrls = new List<Uri>();
            var searchResults = await this._searcher.GetProductsUrls(searchTerm);
            foreach (var searchResult in searchResults)
            {
                productUrls.Add(this.HandleCleanUrl(searchResult));
            }

            return productUrls;
        }
    }
}