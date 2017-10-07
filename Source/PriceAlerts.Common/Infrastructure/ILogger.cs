namespace PriceAlerts.Common.Infrastructure
{
    public interface ILogger
    {
        void LogMessage(string message);
        
        void LogInformation(string message);
        
        void LogWarning(string message);
        
        void LogError(string message);
    }
}