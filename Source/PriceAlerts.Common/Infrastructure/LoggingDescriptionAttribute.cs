using System;

namespace PriceAlerts.Common.Infrastructure
{
    [AttributeUsage(AttributeTargets.Method)]
    public class LoggingDescriptionAttribute : Attribute
    {
        public LoggingDescriptionAttribute(string description)
        {
            this.Description = description;
        }
        
        public string Description { get; }
    }
}