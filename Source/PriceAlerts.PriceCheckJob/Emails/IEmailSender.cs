using System;
using System.Threading.Tasks;

namespace PriceAlerts.PriceCheckJob.Emails
{
    public interface IEmailSender : IDisposable
    {
        Task<ApiTypes.EmailSend> SendEmail(EmailInformation emailInformation);

        void Initialize();
    }
}