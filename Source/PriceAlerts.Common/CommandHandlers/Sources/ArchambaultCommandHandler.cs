using PriceAlerts.Common.Commands.Cleaners.Sources;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Commands.Searchers.Sources;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.CommandHandlers.Sources
{
    internal class ArchambaultCommandHandler : CommandHandler
    {
        public ArchambaultCommandHandler(ArchambaultSource source, 
            ArchambaultParser parser, 
            ArchambaultCleaner cleaner,
            ArchambaultSearcher searcher)
            : base(source)
        {
            this.Commands.Add(parser);
            this.Commands.Add(searcher);
            this.Commands.Add(cleaner);
        }
    }
}