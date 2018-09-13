using System.Net;
using PriceAlerts.Common.Commands.Cleaners;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Commands.LinkManipulators;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.CommandHandlers.Sources
{
    internal class IndigoCommandHandler : CommandHandler
    {
        public IndigoCommandHandler(IndigoSource source, 
            EmptyQueryStringCleaner cleaner, 
            CJLinkManipulator manipulator, 
            IndigoParser parser)
            : base(source)
        {
            this.Commands.Add(cleaner);
            this.Commands.Add(parser);
            this.Commands.Add(manipulator);
        }
    }
}