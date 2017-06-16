using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using PriceAlerts.Common.Parsers;
using PriceAlerts.Common.Parsers.Models;

namespace PriceAlerts.Common.Tests.Parsers
{
    public interface ITestParser : IParser
    {
        Task<IEnumerable<Uri>> GetTestProductsUrls();
    }
}