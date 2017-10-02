using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.CommandHandlers.Sources
{
    internal class IkeaCommandHandler : CommandHandler
    {
        public IkeaCommandHandler(IkeaSource source, IkeaParser parser)
            : base(source)
        {
            this.Commands.Add(parser);
        }
    }
}