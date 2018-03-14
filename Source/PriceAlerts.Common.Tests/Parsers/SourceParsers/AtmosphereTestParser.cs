using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PriceAlerts.Common.Commands.Inspectors.Sources;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Sources;

namespace PriceAlerts.Common.Tests.Parsers.SourceParsers
{
    internal class AtmosphereTestParser : AtmosphereParser, ITestParser
    {
        private readonly IDocumentLoader _documentLoader;

        public AtmosphereTestParser(IDocumentLoader documentLoader)
            : base(documentLoader, new AtmosphereSource())
        {
            this._documentLoader = documentLoader;
        }

        public async Task<IEnumerable<Uri>> GetTestProductsUrls()
        {
            var document = await this._documentLoader.LoadDocument(this.Source.Domain, this.Source.CustomHeaders);

            var urls = new[]
            {
                new Uri(this.Source.Domain, "https://www.atmosphere.ca/categories/men/jackets-vests-pants/mens-rain-shell-jackets/product/columbia-mens-good-ways-jacket-331942645.html#331942645%5Bcolor%5D=41"),
                new Uri(this.Source.Domain, "https://www.atmosphere.ca/categories/footwear/mens-shoes/hiking-shoes/waterproof-hiking-shoes/product/the-north-face-mens-litewave-fastpack-mid-hiking-boots-greybro-332401660.html#332401660%5Bcolor%5D=99"),
                new Uri(this.Source.Domain, "https://www.atmosphere.ca/categories/women/clothing/hoodies/product/the-north-face-womens-tech-sherpa-pullover-hoodie-332355567.html#332355567%5Bcolor%5D=04"),
                new Uri(this.Source.Domain, "https://www.atmosphere.ca/categories/electronics/solar-portable-power/solar-chargers/product/goal-zero-boulder-15-solar-panel-331585440.html#331585440=331585442"),
                new Uri(this.Source.Domain, "https://www.atmosphere.ca/categories/electronics/wearable-watches/activity-trackers/product/fitbit-blaze-fitness-tracker-blacksilver-small-332083004.html#332083004=332083005"),
            };

            return urls;
        }
    }
}