using System;
using System.Collections;
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

            if (invocation.Method.ReturnType == typeof(void))
            {
                return;
            }

            var typeName = invocation.Method.ReturnType.Name;
            var returnValue = invocation.ReturnValue;
            if (invocation.Method.ReturnType.IsGenericType
                && typeof(Task<>).IsAssignableFrom(invocation.Method.ReturnType.GetGenericTypeDefinition()))
            {
                returnValue = invocation.ReturnValue.GetType().GetProperty("Result").GetValue(invocation.ReturnValue);
                typeName = invocation.Method.ReturnType.GetGenericArguments().First().Name;
                
                if (returnValue is IEnumerable returnValues)
                {
                    typeName = $"{returnValue.GetType().GetGenericArguments()[0].Name}[]";
                    returnValue = string.Empty;
                    
                    foreach (var value in returnValues)
                    {
                        returnValue += $"{value}, ";
                    }
            
                    var lastComa = ((string)returnValue).LastIndexOf(", ", StringComparison.Ordinal);
                    if (lastComa >= 0)
                    {
                        returnValue = ((string)returnValue).Substring(0, lastComa);
                    }
                }
            }

            this._logger.LogMessage($"{DateTime.UtcNow.ToLongTimeString()}: Returned {typeName} <{returnValue}>");
        }
    }
}