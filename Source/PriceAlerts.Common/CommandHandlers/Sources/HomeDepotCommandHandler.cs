using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.CommandHandlers.Sources
{
    internal class HomeDepotCommandHandler : CommandHandler
    {
        public HomeDepotCommandHandler(HomeDepotSource source,
            HomeDepotParser parser) 
            : base(source)
        {
            this.Commands.Add(parser);
        }
    }
}