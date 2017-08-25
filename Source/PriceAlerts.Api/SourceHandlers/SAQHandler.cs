using System;

using Autofac;

using PriceAlerts.Api.LinkManipulators.UrlCleaners;
using PriceAlerts.Common;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Searchers.SourceSearchers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Api.SourceHandlers
{
    internal class SAQHandler : BaseHandler, IHandler
    {
        public SAQHandler(SAQSource source, EmptyQueryStringCleaner cleaner, SAQParser parser, EmptySearcher searcher)
            : base(source, cleaner, parser, searcher)
        {
        }
    }
}