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
    internal class SearsHandler : BaseHandler, IHandler
    {
        public SearsHandler(SearsSource source, EmptyQueryStringCleaner cleaner, SearsParser parser, EmptySearcher searcher)
            : base(source, cleaner, parser, searcher)
        {
        }
    }
}