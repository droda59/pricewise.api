using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PriceAlerts.Api.Models;
using PriceAlerts.Common.Database;
using PriceAlerts.Common.Infrastructure;

namespace PriceAlerts.Api.Controllers
{
    [Route("api/[controller]")]
    public class StoreAvailabilityController : Controller
    {
        private readonly IPriceCheckRunStatisticsRepository _priceCheckRunStatisticsRepository;

        public StoreAvailabilityController(IPriceCheckRunStatisticsRepository priceCheckRunStatisticsRepository)
        {
            this._priceCheckRunStatisticsRepository = priceCheckRunStatisticsRepository;
        }
        
        [HttpGet]
        [ResponseCache(Duration = 60 * 60)]
        [LoggingDescription("*** REQUEST to get store availabilities ***")]
        public virtual async Task<IActionResult> GetSharedAlertList()
        {
            var latestStatistics = await this._priceCheckRunStatisticsRepository.GetLatestAsync();
            
            var storeAvailabilities = latestStatistics.Results
                .Select(result => new StoreAvailabilityDto
                {
                    Domain = result.Domain,
                    IsAvailable = 
                        result.Errors + result.Unhandled + result.Successes == 0 
                        || result.Unhandled + result.Successes > 0
                })
                .ToList();

            return this.Ok(storeAvailabilities);
        }
    }
}