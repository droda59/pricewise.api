using System;

using Autofac;

using PriceAlerts.Api.LinkManipulators;
using PriceAlerts.Common;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Searchers.SourceSearchers;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Api.SourceHandlers
{
    internal class AmazonHandler : BaseHandler, IHandler
    {
        public AmazonHandler(AmazonSource source, AmazonLinkManipulator cleaner, AmazonApiParser parser, AmazonSearcher searcher)
            : base(source, cleaner, parser, searcher)
        {
        }
    }
}