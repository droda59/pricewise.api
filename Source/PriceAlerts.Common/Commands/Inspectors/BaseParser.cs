using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PriceAlerts.Common.Models;

namespace PriceAlerts.Common.Commands.Inspectors
{
    public abstract class BaseParser<T> : IParser
        where T : ParserContext
    {
        protected IList<Action> ParserSteps { get; set; }

        public abstract Task<SitePriceInfo> Parse(Uri url);

        protected virtual T Context { get; set; }
    }
}