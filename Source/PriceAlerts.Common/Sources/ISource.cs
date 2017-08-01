using System;
using System.Collections.Generic;

namespace PriceAlerts.Common.Sources
{
    public interface ISource
    {
        Uri Domain { get; }
        
        IEnumerable<KeyValuePair<string, string>> CustomHeaders { get; }
    }
}
