using PriceAlerts.Common.Commands.Cleaners.Sources;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.CommandHandlers.Sources
{
    internal class SportiumCommandHandler : CommandHandler
    {
        public SportiumCommandHandler(SportiumSource source, 
            SportiumCleaner cleaner, 
            SportiumParser parser)
            : base(source)
        {
            this.Commands.Add(cleaner);
            this.Commands.Add(parser);
        }
    }
}