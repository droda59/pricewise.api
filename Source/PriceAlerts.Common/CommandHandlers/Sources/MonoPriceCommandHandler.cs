using PriceAlerts.Common.Commands.Cleaners;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.CommandHandlers.Sources
{
    internal class MonoPriceCommandHandler : CommandHandler
    {
        public MonoPriceCommandHandler(MonoPriceSource source, 
            EmptyQueryStringCleaner cleaner, 
            MonoPriceParser parser)
            : base(source)
        {
            this.Commands.Add(cleaner);
            this.Commands.Add(parser);
        }
    }
}