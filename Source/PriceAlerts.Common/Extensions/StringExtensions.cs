using System;
using System.Linq;
using System.Text;

namespace PriceAlerts.Common
{
    public static class StringExtensions
    {
        public static string ExtractNumber(this string original)
        {
            var numberString = new string(original.Where(c => Char.IsDigit(c) || Char.IsPunctuation(c)).ToArray());

            return numberString;
        }

        public static decimal ExtractDecimal(this string original)
        {
            var extractedValue = original.ExtractNumber();

            if (extractedValue.Contains(","))
            {
                var numberBuilder = new StringBuilder();
                var splitOnComma = extractedValue.Split(',');
                for (var i = 0; i < splitOnComma.Length; i++)
                {
                    var part = splitOnComma[i];

                    if (i == splitOnComma.Length - 1 && !splitOnComma[i].Contains("."))
                    {
                        numberBuilder.Append(".");
                    }

                    numberBuilder.Append(part);
                }

                extractedValue = numberBuilder.ToString();
            } 

            decimal decimalValue = 0m;
            if (!string.IsNullOrWhiteSpace(extractedValue))
            {
                decimalValue = Convert.ToDecimal(extractedValue);
            }

            return decimalValue;
        }
    }
}
