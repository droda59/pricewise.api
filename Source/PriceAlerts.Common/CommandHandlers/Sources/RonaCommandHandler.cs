using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.CommandHandlers.Sources
{
    internal class RonaCommandHandler : CommandHandler
    {
        public RonaCommandHandler(RonaSource source,
            RonaParser parser) 
            : base(source)
        {
            this.Commands.Add(parser);
        }
    }
}