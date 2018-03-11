using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.CommandHandlers.Sources
{
    internal class LaCordeeCommandHandler : CommandHandler
    {
        public LaCordeeCommandHandler(LaCordeeSource source, LaCordeeParser parser)
            : base(source)
        {
            this.Commands.Add(parser);
        }
    }
}