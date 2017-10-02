using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.CommandHandlers.Sources
{
    internal class LeonCommandHandler : CommandHandler
    {
        public LeonCommandHandler(LeonSource source, LeonParser parser)
            : base(source)
        {
            this.Commands.Add(parser);
        }
    }
}