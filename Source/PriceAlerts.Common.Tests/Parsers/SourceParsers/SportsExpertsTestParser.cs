using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class SportsExpertsTestParser : SportsExpertsParser, ITestParser
    {
        private readonly IDocumentLoader _documentLoader;

        public SportsExpertsTestParser(IDocumentLoader documentLoader)
            : base(documentLoader, new SportsExpertsSource())
        {
            this._documentLoader = documentLoader;
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var document = await this._documentLoader.LoadDocument(this.Source.Domain, this.Source.CustomHeaders);

            var urls = new[]
            {
                new Uri(this.Source.Domain, "https://www.sportsexperts.ca/en-CA/p-reflect-medium-adult-shoe-spikes/774852/774852-1"),
                //new Uri(this.Source.Domain, "https://www.mec.ca/en/product/5053-841/%5Bak%5D-BK-Lite-Insulator-Jacket"),
                //new Uri(this.Source.Domain, "https://www.mec.ca/en/product/5058-425/Pro-Wood-Fired-Pizza-Oven"),
                //new Uri(this.Source.Domain, "https://www.mec.ca/en/product/5056-346/Lector-4-9-LC-Bicycle"),
                //new Uri(this.Source.Domain, "https://www.mec.ca/en/product/5036-093/Outpost-Pack"),
            };

            return urls;
        }
    }
}