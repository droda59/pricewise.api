using System;

namespace PriceAlerts.Common.Infrastructure
{
    public class ConsoleLogger : ILogger
    {
        public void LogMessage(string message)
        {
            Console.WriteLine(message);
        }

        public void LogInformation(string message)
        {
            this.LogMessage(message);
        }

        public void LogWarning(string message)
        {
            this.LogMessage(message);
        }

        public void LogError(string message)
        {
            this.LogMessage(message);
        }
    }
}