using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PriceAlerts.Common
{
    public interface IHtmlLoader : IDisposable
    {
        Task<string> ReadHtmlAsync(Uri uri, params KeyValuePair<string, string>[] customHeaders);

        void Initialize();
    }
}
