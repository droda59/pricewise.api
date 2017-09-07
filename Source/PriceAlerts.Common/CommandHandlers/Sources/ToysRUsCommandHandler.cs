using PriceAlerts.Common.Commands.Cleaners.Sources;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.CommandHandlers.Sources
{
    internal class ToysRUsCommandHandler : CommandHandler
    {
        public ToysRUsCommandHandler(ToysRUsSource source, 
            ToysRUsCleaner cleaner, 
            ToysRUsParser parser)
            : base(source)
        {
            this.Commands.Add(cleaner);
            this.Commands.Add(parser);
        }
    }
}