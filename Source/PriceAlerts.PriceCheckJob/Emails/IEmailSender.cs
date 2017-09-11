using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PriceAlerts.PriceCheckJob.Emails
{
    public interface IEmailSender : IDisposable
    {
        Task<ApiTypes.EmailSend> SendEmail(string emailAddress, IDictionary<string, string> parameters, string templateName);

        void Initialize();
    }
}