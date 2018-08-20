using System;
using HtmlAgilityPack;

namespace PriceAlerts.Common.Commands.Inspectors
{
    public class HtmlParserContext : ParserContext
    {
        public HtmlParserContext(Uri sourceUri, HtmlDocument document)
            : base(sourceUri)
        {
            this.Document = document;
        }

        public HtmlDocument Document { get; }
    }
}