using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using HtmlAgilityPack;

namespace PriceAlerts.Common.Infrastructure
{
    public interface IDocumentLoader
    {
        Task<HtmlDocument> LoadDocument(Uri url, IEnumerable<KeyValuePair<string, string>> customHeaders);
    }
}