using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.CommandHandlers.Sources
{
    internal class MECCommandHandler : CommandHandler
    {
        public MECCommandHandler(MECSource source, MECParser parser)
            : base(source)
        {
            this.Commands.Add(parser);
        }
    }
}