using PriceAlerts.Common.Commands.Cleaners.Sources;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Commands.Searchers.Sources;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.CommandHandlers.Sources
{
    internal class CarcajouCommandHandler : CommandHandler
    {
        public CarcajouCommandHandler(CarcajouSource source, 
            CarcajouCleaner cleaner, 
            CarcajouParser parser, 
            CarcajouSearcher searcher)
            : base(source)
        {
            this.Commands.Add(cleaner);
            this.Commands.Add(parser);
            this.Commands.Add(searcher);
        }
    }
}