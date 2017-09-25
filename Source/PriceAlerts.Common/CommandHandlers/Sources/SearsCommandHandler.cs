using PriceAlerts.Common.Commands.Cleaners;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.CommandHandlers.Sources
{
    internal class SearsCommandHandler : CommandHandler
    {
        public SearsCommandHandler(SearsSource source, 
            AnchorCleaner anchorCleaner, 
            EmptyQueryStringCleaner queryStringCleaner, 
            SearsParser parser)
            : base(source)
        {
            this.Commands.Add(anchorCleaner);
            this.Commands.Add(queryStringCleaner);
            this.Commands.Add(parser);
        }
    }
}