using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Autofac;
using Microsoft.Extensions.Logging;
using PriceAlerts.Common.Tests.Parsers;
using PriceAlerts.Common.Tests.Parsers.SourceParsers;

using Xunit;

namespace PriceAlerts.Common.Tests
{
    public class SourceParserTests
    {
        private readonly IContainer _container;

        public SourceParserTests()
        {
			var builder = new ContainerBuilder();
            builder.RegisterModule(new AutofacModule());
            builder.Register<ILoggerFactory>(c => new LoggerFactory());

            this._container = builder.Build();
        }

        [Theory]
        [InlineData(typeof(AmazonTestParser))]
        [InlineData(typeof(ArchambaultTestParser))]
        [InlineData(typeof(BestBuyTestParser))]
        [InlineData(typeof(BraultMartineauTestParser))]
        [InlineData(typeof(CanadianTireTestParser))]
        [InlineData(typeof(CarcajouTestParser))]
        [InlineData(typeof(HomeDepotTestParser))]
        [InlineData(typeof(IkeaTestParser))]
        [InlineData(typeof(IndigoTestParser))]
        [InlineData(typeof(LegoTestParser))]
        [InlineData(typeof(LeonTestParser))]
        [InlineData(typeof(MonoPriceTestParser))]
        [InlineData(typeof(NcixTestParser))]
        [InlineData(typeof(RenaudBrayTestParser))]
        [InlineData(typeof(SAQTestParser))]
        [InlineData(typeof(StaplesTestParser))]
        [InlineData(typeof(TigerDirectTestParser))]
        [InlineData(typeof(ToysRUsTestParser))]
        public async Task GetSiteInfo_AlwaysReturnSiteInfo(Type parserType)
        {
            var parser = this.CreateTestParser(parserType);
            var urlsToTest = (await parser.GetTestProductsUrls()).ToList();

            Assert.NotEmpty(urlsToTest);
            foreach (var urlToTest in urlsToTest)
            {
                Console.WriteLine($"Testing {urlToTest.AbsoluteUri}");

                try
                {
                    var siteInfo = await parser.GetSiteInfo(urlToTest);

                    Assert.NotNull(siteInfo);
                    Assert.NotNull(siteInfo.Uri);
                    Assert.NotNull(siteInfo.Title);
                    Assert.NotNull(siteInfo.ImageUrl);
                    Assert.NotNull(siteInfo.ProductIdentifier);
                    Assert.True(siteInfo.Price >= 0);
                }
                catch (Exception)
                {
                    Console.WriteLine($"Failed test on {urlToTest}");

                    throw;
                }
            }

            Console.WriteLine($"Ran {urlsToTest.Count} tests on {parserType.Name.Replace("TestParser", string.Empty)}");
            Console.WriteLine();
        }

        private ITestParser CreateTestParser(Type parserType)
        {
            var ctor = parserType.GetConstructors()[0];
            var parameters = new List<object>();
            var parameterInfos = ctor.GetParameters();

            foreach (var parameterInfo in parameterInfos)
            {
                parameters.Add(this._container.Resolve(parameterInfo.ParameterType));
            }

            return (ITestParser)ctor.Invoke(parameters.ToArray());
        }
    }
}
