using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Autofac;

using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Parsers;
using PriceAlerts.Common.Parsers.SourceParsers;
using PriceAlerts.Common.Sources;
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
            builder.RegisterModule(new PriceAlerts.Common.AutofacModule());

            this._container = builder.Build();
        }

        [Theory]
        [InlineData(typeof(AmazonTestParser))]
        [InlineData(typeof(ArchambaultTestParser))]
        [InlineData(typeof(BestBuyTestParser))]
        [InlineData(typeof(BraultMartineauTestParser))]
        [InlineData(typeof(CanadianTireTestParser))]
        [InlineData(typeof(CarcajouTestParser))]
        // [InlineData(typeof(IndigoTestParser))]
        [InlineData(typeof(LegoTestParser))]
        [InlineData(typeof(LeonTestParser))]
        // [InlineData(typeof(NeweggTestParser))]
        [InlineData(typeof(RenaudBrayTestParser))]
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

                    // Console.WriteLine($"Product ID {siteInfo.ProductIdentifier}");

                    Assert.NotNull(siteInfo);
                    Assert.NotNull(siteInfo.Uri);
                    Assert.NotNull(siteInfo.Title);
                    Assert.NotNull(siteInfo.ImageUrl);
                    Assert.NotNull(siteInfo.ProductIdentifier);
                    Assert.True(siteInfo.Price >= 0);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed test on {urlToTest}");

                    throw e;
                }
            }

            Console.WriteLine($"Ran {urlsToTest.Count} tests on {parser.Source.Domain}");
            Console.WriteLine();
        }

        private ITestParser CreateTestParser(Type parserType)
        {
            ConstructorInfo ctor = parserType.GetConstructors()[0];
            List<object> parameters = new List<object>();
            var parameterInfos = ctor.GetParameters();

            foreach (ParameterInfo parameterInfo in parameterInfos)
            {
                parameters.Add(this._container.Resolve(parameterInfo.ParameterType));
            }

            return (ITestParser)ctor.Invoke(parameters.ToArray());
        }
    }
}
