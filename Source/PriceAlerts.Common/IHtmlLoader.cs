using System;
using System.Threading.Tasks;

namespace PriceAlerts.Common
{
    public interface IHtmlLoader : IDisposable
    {
        Task<string> ReadHtmlAsync(Uri uri);

        void Initialize();
    }
}
