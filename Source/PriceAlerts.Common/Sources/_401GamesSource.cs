using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PriceAlerts.Common.Sources
{
    public class _401GamesSource : ShopifySource
    {
        public _401GamesSource()
            : base("https://store.401games.ca")
        {
        }
    }
}