using PriceAlerts.Common.Commands.Cleaners.Sources;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.CommandHandlers.Sources
{
    internal class TigerDirectCommandHandler : CommandHandler
    {
        public TigerDirectCommandHandler(TigerDirectSource source, 
            TigerDirectCleaner cleaner, 
            TigerDirectParser parser)
            : base(source)
        {
            this.Commands.Add(cleaner);
            this.Commands.Add(parser);
        }
    }
}