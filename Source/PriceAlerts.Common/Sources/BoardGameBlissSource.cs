using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PriceAlerts.Common.Sources
{
    public class BoardGameBlissSource : ShopifySource
    {
        public BoardGameBlissSource()
            : base("https://www.boardgamebliss.com")
        {
            
        }
    }
}