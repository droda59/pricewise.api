using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class WalmartTestParser : WalmartParser, ITestParser
    {
        public WalmartTestParser(IDocumentLoader documentLoader)
            : base(documentLoader, new WalmartSource())
        {
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            return new List<Uri>{ new Uri("https://www.walmart.ca/en/ip/kidkraft-uptown-espresso-kitchen/6000051735102") };
        }
    }
}
