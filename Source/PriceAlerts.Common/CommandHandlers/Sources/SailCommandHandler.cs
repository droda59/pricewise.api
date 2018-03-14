using PriceAlerts.Common.Commands.Cleaners.Sources;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.CommandHandlers.Sources
{
    internal class SailCommandHandler : CommandHandler
    {
        public SailCommandHandler(SailSource source, SailParser parser)
            : base(source)
        {
            this.Commands.Add(parser);
        }
    }
}