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
                    var parameterValue = stringParameters[i]?.ToString();
                    
                    if (!(stringParameters[i] is string) && stringParameters[i] is IEnumerable parameterValues)
                    {
                        parameterValue = string.Empty;
                        foreach (var value in parameterValues)
                        {
                            parameterValue += $"{value}, ";
                        }
        
                        parameterValue = RemoveLastComma(parameterValue);
                    }
                    
                    description += $"<{invocation.Method.GetParameters()[i].Name}: {parameterValue ?? string.Empty}>, ";
                }
            
                description = RemoveLastComma(description);
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
                    typeName = $"{returnValue.GetType().GetGenericArguments()[0]?.Name}[]";
                    returnValue = string.Empty;
                    
                    foreach (var value in returnValues)
                    {
                        returnValue += $"{value}, ";
                    }
            
                    returnValue = RemoveLastComma((string)returnValue);
                }
            }

            this._logger.LogMessage($"{DateTime.UtcNow.ToLongTimeString()}: Returned from {invocation.TargetType.Name} with {typeName} <{returnValue}>");
        }

        private static string RemoveLastComma(string returnValue)
        {
            var valueWithoutComma = returnValue;
            var lastComa = returnValue.LastIndexOf(", ", StringComparison.Ordinal);
            if (lastComa >= 0)
            {
                valueWithoutComma = valueWithoutComma.Substring(0, lastComa);
            }
            
            return valueWithoutComma;
        }
    }
}