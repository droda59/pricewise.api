using System;
using System.Threading.Tasks;
using PriceAlerts.Api.Models;
using PriceAlerts.Common.Infrastructure;
using PriceAlerts.Common.Models;

namespace PriceAlerts.Api.Factories
{
    public interface IAlertListFactory
    {
        [LoggingDescription("Create alert list model")]
        Task<ListDto> CreateAlertList(List repoList);

        [LoggingDescription("Create alert list model")]
        Task<TList> CreateAlertList<TList>(List list, Func<UserAlert, bool> alertFilter)
            where TList : ListDto, new();
    }
}