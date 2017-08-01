using System;
using System.Text.RegularExpressions;

namespace PriceAlerts.Common
{
    public static class ISBNHelper
    {
        public static readonly Regex ISBN13Expression = new Regex(@"[0-9]{13}", RegexOptions.Compiled);
        
        public static readonly Regex ISBN13CompleteExpression = new Regex(@"[0-9]{3}\-[0-9]{10}", RegexOptions.Compiled);
        
        public static readonly Regex ISBN10Expression = new Regex(@"[0-9]{10}", RegexOptions.Compiled);
    }
}
