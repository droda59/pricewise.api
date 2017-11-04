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
        private readonly IDocumentLoader _documentLoader;

        public WalmartTestParser(IDocumentLoader documentLoader)
            : base(documentLoader, new WalmartSource())
        {
            this._documentLoader = documentLoader;
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var document = await this._documentLoader.LoadDocument(new Uri(this.Source.Domain, "https://www.walmart.ca/fr/ip/systme-dinfusion-domestique-une-tasse-pour-boisson-multiple-tassimo-t47-de-bosch/6000195955620"), this.Source.CustomHeaders);

            return new List<Uri>{ new Uri("https://www.walmart.ca/fr/ip/systme-dinfusion-domestique-une-tasse-pour-boisson-multiple-tassimo-t47-de-bosch/6000195955620") };
        }
    }
}
