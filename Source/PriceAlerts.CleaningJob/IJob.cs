using System.Threading.Tasks;

namespace PriceAlerts.CleaningJob
{
    internal interface IJob
    {
        Task ExecuteJob();
    }
}