using System.Collections.Generic;
using System.Threading.Tasks;
<<<<<<< HEAD
=======
using PriceAlerts.Common.Infrastructure;
>>>>>>> master
using PriceAlerts.Common.Models;

namespace PriceAlerts.Common.Database
{
    public interface IPriceCheckRunStatisticsRepository
    {
<<<<<<< HEAD
        Task<PriceCheckRunStatistics> GetLatestAsync();
        
=======
        [LoggingDescription("Get latest price check run statistics")]
        Task<PriceCheckRunStatistics> GetLatestAsync();
        
        [LoggingDescription("Insert new price check run statistics")]
>>>>>>> master
        Task<PriceCheckRunStatistics> InsertAsync(IEnumerable<PriceCheckRunDomainStatistics> statistics);
    }
}