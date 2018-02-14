using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using PriceAlerts.Common.Models;

namespace PriceAlerts.Common.Database
{
    internal class PriceCheckRunStatisticsRepository : EntityRepository<PriceCheckRunStatistics>, IPriceCheckRunStatisticsRepository
    {
        public async Task<PriceCheckRunStatistics> GetLatestAsync()
        {
            return await this.Collection.Find(FilterDefinition<PriceCheckRunStatistics>.Empty).SortByDescending(x => x.RunAt).FirstOrDefaultAsync();
        }
        
        public async Task<PriceCheckRunStatistics> InsertAsync(IEnumerable<PriceCheckRunDomainStatistics> statistics)
        {
            var priceCheckRun = new PriceCheckRunStatistics
            {
                RunAt = DateTime.UtcNow,
                Results = statistics
            };

            await this.Collection.InsertOneAsync(priceCheckRun);

            return await this.GetLatestAsync();
        }
    }
}