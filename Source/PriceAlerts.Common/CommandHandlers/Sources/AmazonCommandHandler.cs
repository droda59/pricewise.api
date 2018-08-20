using PriceAlerts.Common.Commands.Cleaners.Sources;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Commands.LinkManipulators.Sources;
using PriceAlerts.Common.Commands.Searchers.Sources;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.CommandHandlers.Sources
{
    internal class AmazonCommandHandler : CommandHandler
    {
        public AmazonCommandHandler(AmazonSource source, 
            AmazonLinkManipulator manipulator,
            AmazonCleaner cleaner,
            AmazonApiParser apiParser, 
            AmazonHtmlParser htmlParser, 
            AmazonApiSearcher searcher)
            : base(source)
        {
            this.Commands.Add(manipulator);
            this.Commands.Add(cleaner);
            this.Commands.Add(apiParser);
            this.Commands.Add(htmlParser);
            this.Commands.Add(searcher);
        }
    }
}