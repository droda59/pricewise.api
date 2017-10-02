using PriceAlerts.Common.Commands.Cleaners;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Commands.LinkManipulators.Sources;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.CommandHandlers.Sources
{
    internal class LegoCommandHandler : CommandHandler
    {
        public LegoCommandHandler(LegoSource source, 
            EmptyQueryStringCleaner cleaner, 
            LegoLinkManipulator manipulator, 
            LegoParser parser)
            : base(source)
        {
            this.Commands.Add(cleaner);
            this.Commands.Add(manipulator);
            this.Commands.Add(parser);
        }
    }
}