using PriceAlerts.Common.Commands.Cleaners;
using PriceAlerts.Common.Commands.Cleaners.Sources;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.CommandHandlers.Sources
{
    internal class AtmosphereCommandHandler : CommandHandler
    {
        public AtmosphereCommandHandler(AtmosphereSource source, AtmosphereParser parser)
            : base(source)
        {
            this.Commands.Add(parser);
        }
    }
}