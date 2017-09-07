using System;

using Autofac;
using PriceAlerts.Common;
using PriceAlerts.Common.LinkManipulators.UrlCleaners;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Searchers.SourceSearchers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.SourceHandlers
{
    internal class BraultMartineauHandler : BaseHandler, IHandler
    {
        public BraultMartineauHandler(BraultMartineauSource source, ReturnOriginalCleaner cleaner, BraultMartineauParser parser, EmptySearcher searcher)
            : base(source, cleaner, parser, searcher)
        {
        }
    }
}