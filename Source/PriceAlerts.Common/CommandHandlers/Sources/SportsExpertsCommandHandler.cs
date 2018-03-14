using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.CommandHandlers.Sources
{
    internal class SportsExpertsCommandHandler : CommandHandler
    {
        public SportsExpertsCommandHandler(SportsExpertsSource source, SportsExpertsParser parser)
            : base(source)
        {
            this.Commands.Add(parser);
        }
    }
}