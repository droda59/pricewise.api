using PriceAlerts.Common.Commands.Cleaners.Sources;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Commands.Searchers.Sources;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.CommandHandlers.Sources
{
    internal class RenaudBrayCommandHandler : CommandHandler
    {
        public RenaudBrayCommandHandler(RenaudBraySource source, 
            RenaudBrayCleaner cleaner, 
            RenaudBrayParser parser, 
            RenaudBraySearcher searcher)
            : base(source)
        {
            this.Commands.Add(cleaner);
            this.Commands.Add(parser);
            this.Commands.Add(searcher);
        }
    }
}