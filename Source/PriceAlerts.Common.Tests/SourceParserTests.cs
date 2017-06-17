using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using PriceAlerts.Common.Parsers;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Tests.Parsers;
using PriceAlerts.Common.Tests.Parsers.SourceParsers;

using Xunit;

namespace PriceAlerts.Common.Tests
{
    public class SourceParserTests
    {
        private readonly HtmlLoader _htmlLoader;

        private IParser _parser;

        public SourceParserTests()
        {
            this._htmlLoader = new HtmlLoader();
        }

        [Theory]
        // [InlineData(typeof(AmazonTestParser))]
        [InlineData(typeof(ArchambaultTestParser))]
        // [InlineData(typeof(NeweggTestParser))]
        public async Task GetSiteInfo_AlwaysReturnSiteInfo(Type parserType)
        {
            this._parser = this.CreateTestParser(parserType, this._htmlLoader);
            var urlsToTest = await this.GetUrlsToTest();

            Assert.NotEmpty(urlsToTest);
            foreach (var urlToTest in urlsToTest)
            {
                Console.WriteLine($"Testing {urlToTest.AbsoluteUri}");

                var siteInfo = await this._parser.GetSiteInfo(urlToTest);

                Assert.NotNull(siteInfo);
                Assert.NotNull(siteInfo.Uri);
                Assert.NotNull(siteInfo.Title);
                Assert.NotNull(siteInfo.ImageUrl);
                Assert.True(siteInfo.Price >= 0);
            }
        }

        private ITestParser CreateTestParser(Type parserType, IHtmlLoader htmlLoader)
        {
            return (ITestParser)Activator.CreateInstance(parserType, htmlLoader);
        }

        private async Task<IEnumerable<Uri>> GetUrlsToTest()
        {
            return await ((ITestParser)this._parser).GetTestProductsUrls();
        }
    }
}
