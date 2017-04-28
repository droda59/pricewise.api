using System;
using System.Threading.Tasks;

using PriceAlerts.PriceCheckJob.Models;

namespace PriceAlerts.PriceCheckJob.Emails
{
    public interface IEmailSender : IDisposable
    {
        Task<ApiTypes.EmailSend> SendEmail(PriceChangeAlert alert);

        void Initialize();
    }
}