using System.Collections.Generic;
using System.Threading.Tasks;
using PriceAlerts.Common.Models;

namespace PriceAlerts.Common.Database
{
    public interface IPriceCheckRunStatisticsRepository
    {
        Task<PriceCheckRunStatistics> GetLatestAsync();
        
        Task<PriceCheckRunStatistics> InsertAsync(IEnumerable<PriceCheckRunDomainStatistics> statistics);
    }
}