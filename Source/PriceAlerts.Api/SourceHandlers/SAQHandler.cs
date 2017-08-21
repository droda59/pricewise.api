using System;

using Autofac;

using PriceAlerts.Api.UrlCleaners;
using PriceAlerts.Api.UrlCleaners.Sources;
using PriceAlerts.Common;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Searchers.SourceSearchers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Api.SourceHandlers
{
    internal class SAQHandler : BaseHandler, IHandler
    {
        public SAQHandler(SAQSource source, OriginalCleaner cleaner, SAQParser parser, EmptySearcher searcher)
            : base(source, cleaner, parser, searcher)
        {
        }
    }
}