using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.CommandHandlers.Sources
{
    internal class SportiumCommandHandler : CommandHandler
    {
        public SportiumCommandHandler(SportiumSource source, SportiumParser parser)
            : base(source)
        {
            this.Commands.Add(parser);
        }
    }
}