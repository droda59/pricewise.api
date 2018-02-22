using System.Collections.Generic;
using System.Threading.Tasks;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Models;

namespace PriceAlerts.Common.Database
{
    public interface IPriceCheckRunStatisticsRepository
    {
        [LoggingDescription("Get latest price check run statistics")]
        Task<PriceCheckRunStatistics> GetLatestAsync();
        
        [LoggingDescription("Insert new price check run statistics")]
        Task<PriceCheckRunStatistics> InsertAsync(IEnumerable<PriceCheckRunDomainStatistics> statistics);
    }
}