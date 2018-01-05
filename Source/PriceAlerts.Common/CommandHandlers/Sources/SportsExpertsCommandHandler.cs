using PriceAlerts.Common.Commands.Cleaners.Sources;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.CommandHandlers.Sources
{
    internal class SportsExpertsCommandHandler : CommandHandler
    {
        public SportsExpertsCommandHandler(SportsExpertsSource source, 
            SportsExpertsCleaner cleaner, 
            SportsExpertsParser parser)
            : base(source)
        {
            this.Commands.Add(cleaner);
            this.Commands.Add(parser);
        }
    }
}