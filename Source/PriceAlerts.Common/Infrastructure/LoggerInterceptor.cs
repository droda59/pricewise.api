using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace PriceAlerts.Common.Infrastructure
{
    public class LoggerInterceptor : IInterceptor
    {
        private readonly ILogger _logger;

        public LoggerInterceptor(ILogger logger)
        {
            this._logger = logger;
        }

        public void Intercept(IInvocation invocation)
        {
            var description = invocation.Method.GetCustomAttribute<LoggingDescriptionAttribute>()?.Description ?? invocation.Method.Name;
            var stringParameters = invocation.Arguments.ToArray();
            if (stringParameters.Any())
            {
                description += " with parameters ";
                
                for (var i = 0; i < stringParameters.Length; i++)
                {
                    description += $"<{invocation.Method.GetParameters()[i].Name}: {stringParameters[i] ?? string.Empty}>, ";
                }
            
                var lastComa = description.LastIndexOf(", ", StringComparison.Ordinal);
                if (lastComa >= 0)
                {
                    description = description.Substring(0, lastComa);
                }
            }
            
            this._logger.LogMessage($"{DateTime.UtcNow.ToLongTimeString()}: {description}");

            try
            {
                invocation.Proceed();                
            }
            catch (Exception e)
            {
                this._logger.LogError($"{DateTime.UtcNow.ToLongTimeString()}: !!! Logged Exception: {e.Message}");
                throw;
            }

            var returnValue = invocation.ReturnValue;
            if (invocation.Method.ReturnType == typeof(void))
            {
                return;
            }
            
            if (invocation.Method.ReturnType.IsGenericType
                && typeof(Task<>).IsAssignableFrom(invocation.Method.ReturnType.GetGenericTypeDefinition()))
            {
                returnValue = invocation.ReturnValue.GetType().GetProperty("Result")
                    .GetValue(invocation.ReturnValue);
            }

            var typeName = invocation.Method.ReturnType.IsGenericType 
                ? invocation.Method.ReturnType.GetGenericArguments().First().Name 
                : invocation.Method.ReturnType.Name;

            this._logger.LogMessage($"{DateTime.UtcNow.ToLongTimeString()}: Returned {typeName} <{returnValue}>");
        }
    }
}