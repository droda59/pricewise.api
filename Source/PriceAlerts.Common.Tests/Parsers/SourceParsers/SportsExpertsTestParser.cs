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
                new Uri(this.Source.Domain, "https://www.sportsexperts.ca/fr-CA/p-flex-trainer-8-chaussures-dentrainement-pour-femme/275547/275547-8"),
                new Uri(this.Source.Domain, "https://www.sportsexperts.ca/fr-CA/p-air-zoom-pegasus-35-chaussures-de-course-a-pied-pour-homme/275531/275531-111"),
                new Uri(this.Source.Domain, "https://www.sportsexperts.ca/fr-CA/p-zne-parley-blouson-dentrainement-pour-femme/327962/327962-3"),
                new Uri(this.Source.Domain, "https://www.sportsexperts.ca/fr-CA/p-cuivre-socquettes-de-compression-pour-homme/304991/304991-2"),
                new Uri(this.Source.Domain, "https://www.sportsexperts.ca/fr-CA/p-free-trainer-v8-chaussures-dentrainement-pour-homme/275489/275489-53"),

                // Products found through the Atmosphere QC website just in case
                new Uri(this.Source.Domain, "https://www.sportsexperts.ca/fr-CA/p-malibu-pedal-kayak-recreatif/285474/285474-1"),
                new Uri(this.Source.Domain, "https://www.sportsexperts.ca/en-CA/p-raven-20l-backpack/12546/12546-6"),
            };

            return urls;
        }
    }
}