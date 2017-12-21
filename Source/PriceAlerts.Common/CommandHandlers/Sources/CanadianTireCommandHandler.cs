using PriceAlerts.Common.Commands.Cleaners;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.CommandHandlers.Sources
{
    internal class CanadianTireCommandHandler : CommandHandler
    {
        public CanadianTireCommandHandler(CanadianTireSource source, 
            EmptyQueryStringCleaner cleaner, 
            AnchorCleaner anchorCleaner, 
            CanadianTireParser parser)
            : base(source)
        {
            this.Commands.Add(cleaner);
            this.Commands.Add(anchorCleaner);
            this.Commands.Add(parser);
        }
    }
}