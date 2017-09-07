using System;

using Autofac;
using PriceAlerts.Common;
using PriceAlerts.Common.LinkManipulators.UrlCleaners;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Searchers.SourceSearchers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.SourceHandlers
{
    internal class SearsHandler : BaseHandler, IHandler
    {
        public SearsHandler(SearsSource source, EmptyQueryStringCleaner cleaner, SearsParser parser, EmptySearcher searcher)
            : base(source, cleaner, parser, searcher)
        {
        }
    }
}