using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PriceAlerts.Common.Commands.Inspectors;

namespace PriceAlerts.Common.Tests.Parsers
{
    internal interface ITestParser : IInspector
    {
        Task<IEnumerable<Uri>> GetTestProductsUrls();
    }
}