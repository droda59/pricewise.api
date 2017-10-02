using PriceAlerts.Common.Commands.Cleaners;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.CommandHandlers.Sources
{
    internal class SAQCommandHandler : CommandHandler
    
    {
        public SAQCommandHandler(SAQSource source, 
            EmptyQueryStringCleaner cleaner, 
            SAQParser parser)
            : base(source)
        {
            this.Commands.Add(cleaner);
            this.Commands.Add(parser);
        }
    }
}