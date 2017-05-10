using System;

using Xunit;

namespace PriceAlerts.Common.Tests
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("not a number at all")]
        [InlineData("contains spaces                           ")]
        [InlineData("contains non punctation characters $+=±£¢¤¬¦²³¼¾~<>")]
        public void ExtractNumber_ContainsNoNumber_EmptyString(string original)
        {
            var numberString = original.ExtractNumber();

            Assert.Equal(string.Empty, numberString);
        }

        [Theory]
        [InlineData("12")]
        [InlineData(":;!?,.()[]{}*\\\"#/%&_-@")]
        [InlineData("12,34,45")]
        [InlineData("12,345,678.90")]
        public void ExtractNumber_ContainsOnlyNumberAndPunctation(string original)
        {
            var numberString = original.ExtractNumber();

            Assert.Equal(original, numberString);
        }

        [Theory]
        [InlineData("", 0)]
        [InlineData("this is a number 12", 12)]
        [InlineData("12,345,678.90", 12345678.90)]
        [InlineData("12 345 678,90", 12345678.90)]
        [InlineData("12 345 678.90", 12345678.90)]
        [InlineData("$ 123.45", 123.45)]
        [InlineData("£ 123.45", 123.45)]
        [InlineData("¢ 123.45", 123.45)]
        [InlineData("123.45", 123.45)]
        [InlineData("123,45", 123.45)]
        [InlineData("123.00", 123)]
        [InlineData("123,00", 123)]
        [InlineData("123", 123)]
        public void ExtractDecimal(string original, decimal result)
        {
            var number = original.ExtractDecimal();

            Assert.Equal(result, number);
        }
    }
}
