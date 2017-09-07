using PriceAlerts.Common.Commands.Cleaners.Sources;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.CommandHandlers.Sources
{
    internal class BestBuyCommandHandler : CommandHandler
    {
        public BestBuyCommandHandler(BestBuySource source, 
            BestBuyCleaner cleaner, 
            BestBuyParser parser)
            : base(source)
        {
            this.Commands.Add(cleaner);
            this.Commands.Add(parser);
        }
    }
}