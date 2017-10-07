using System.Diagnostics;

namespace PriceAlerts.Common.Infrastructure
{
    public class TraceLogger : ILogger
    {
        public void LogMessage(string message)
        {
            Trace.WriteLine(message);
        }

        public void LogInformation(string message)
        {
            Trace.TraceInformation(message);
        }

        public void LogWarning(string message)
        {
            Trace.TraceWarning(message);
        }

        public void LogError(string message)
        {
            Trace.TraceError(message);
        }
    }
}