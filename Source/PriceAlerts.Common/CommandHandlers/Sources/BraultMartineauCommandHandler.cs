using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.CommandHandlers.Sources
{
    internal class BraultMartineauCommandHandler : CommandHandler
    {
        public BraultMartineauCommandHandler(BraultMartineauSource source, BraultMartineauParser parser)
            : base(source)
        {
            this.Commands.Add(parser);
        }
    }
}