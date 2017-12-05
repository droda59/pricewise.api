using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceAlerts.Api.Models;
using PriceAlerts.Common.Models;

namespace PriceAlerts.Api.Factories
{
    internal class AlertListFactory : IAlertListFactory
    {
        private readonly IUserAlertFactory _userAlertFactory;

        public AlertListFactory(IUserAlertFactory userAlertFactory)
        {
            this._userAlertFactory = userAlertFactory;
        }
        
        public async Task<ListDto> CreateAlertList(List repoList)
        {
            var lockObject = new object();
            var summaries = new List<UserAlertSummaryDto>();
            await Task.WhenAll(repoList.Alerts.Select(async alert =>
            {
                var summary = await this._userAlertFactory.CreateUserAlertSummary(alert);
                lock (lockObject)
                {
                    summaries.Add(summary);
                }
            }));

            var listDto = new ListDto
            {
                Id = repoList.Id,
                Name = repoList.Name,
                Alerts = summaries
            };
            
            return listDto;
        }
    }
}