using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceAlerts.Common.Commands;
using PriceAlerts.Common.Commands.Cleaners;
using PriceAlerts.Common.Commands.Inspectors;
using PriceAlerts.Common.Commands.LinkManipulators;
using PriceAlerts.Common.Commands.Searchers;
using PriceAlerts.Common.Models;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.CommandHandlers
{
    internal abstract class CommandHandler : ICommandHandler
    {
        private readonly ISource _source;

        protected CommandHandler(ISource source)
        {
            this._source = source;
            
            this.Commands = new List<ICommand>();
        }

        Uri ICommandHandler.Domain => this._source.Domain;

        protected IList<ICommand> Commands { get; }
        
        public Uri HandleCleanUrl(Uri url)
        {
            return this.Commands.OfType<ICleaner>().Aggregate(url, (current, cleaner) => cleaner.CleanUrl(current));
        }

        public Uri HandleManipulateUrl(Uri url)
        {
            return this.Commands.OfType<ILinkManipulator>().Aggregate(url, (current, manipulator) => manipulator.ManipulateLink(current));
        }

        public async Task<SitePriceInfo> HandleGetInfo(Uri url)
        {
            SitePriceInfo siteInfo = null;
            var cleanUrl = this.HandleCleanUrl(url);

            var currentIndex = 0;
            var parsers = this.Commands.OfType<IInspector>().ToList();
            while (siteInfo == null && currentIndex < parsers.Count)
            {
                try 
                {
                    siteInfo = await parsers[currentIndex].GetSiteInfo(cleanUrl);
                }
                catch (Exception)
                {
                    // ignored
                }
                finally
                {
                    currentIndex++;
                }
            }

            if (siteInfo != null)
            {
                siteInfo.Uri = cleanUrl.AbsoluteUri;
            }

            return siteInfo;
        }

        public async Task<IEnumerable<Uri>> HandleSearch(string searchTerm)
        {
            var productUrls = new List<Uri>();
            foreach (var searcher in this.Commands.OfType<ISearcher>())
            {
                var foundUrls = await searcher.GetProductsUrls(searchTerm);
                productUrls.AddRange(foundUrls.Select(this.HandleCleanUrl));
            }

            return productUrls;
        }
    }
}