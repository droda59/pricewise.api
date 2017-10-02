using System;
using System.Linq;
using System.Text;

namespace PriceAlerts.Common.Extensions
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

        public static bool IsBase64Url(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var base64string = value.Trim().Replace("\\n", string.Empty);

            if (!base64string.StartsWith("data:image/png;base64,")
            && !base64string.StartsWith("data:image/jpeg;base64,"))
            {
                return false;
            }

            base64string = base64string
                .Replace("data:image/png;base64,", string.Empty)
                .Replace("data:image/jpeg;base64,", string.Empty);

            if (base64string.Length % 4 != 0)
            {
                return false;
            }

            // if (!Regex.IsMatch(base64string, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None))
            // {
            //     return false;
            // }

            try
            {
                Convert.FromBase64String(base64string);
            }
            catch(Exception)
            {
                return false;
            }

            return true;
        }
    }
}
