using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Microsoft.Extensions.Logging;

namespace PriceAlerts.Common.Infrastructure
{
    public class LoggerInterceptor : IInterceptor
    {
        private readonly ILogger _logger;

        public LoggerInterceptor(ILoggerFactory loggerFactory)
        {
            this._logger = loggerFactory.CreateLogger("Logging");
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
                    description += GetParameterDescription(stringParameters[i], invocation.Method.GetParameters()[i]);
                    if (i < stringParameters.Length - 1)
                    {
                        description += ", ";
                    }
                }
            }
            
            this._logger.LogInformation($"{DateTime.UtcNow.ToLongTimeString()}: {description}");

            try
            {
                invocation.Proceed();

                if (invocation.Method.ReturnType == typeof(void))
                {
                    return;
                }

                var typeName = invocation.Method.ReturnType.Name;
                var returnValue = invocation.ReturnValue;
                if (invocation.Method.ReturnType.IsGenericType
                    && typeof(Task<>).IsAssignableFrom(invocation.Method.ReturnType.GetGenericTypeDefinition()))
                {
                    var task = (Task)invocation.ReturnValue;
                    task.ContinueWith(t => 
                    {
                        if (t.IsFaulted)
                        {
                            this._logger.LogError(t.Exception, $"{DateTime.UtcNow.ToLongTimeString()}: !!! Logged Exception: {t.Exception.Message}");
                            throw t.Exception;
                        }
                        
                        returnValue = t.GetType().GetProperty("Result").GetValue(invocation.ReturnValue);
                        typeName = invocation.Method.ReturnType.GetGenericArguments().First().Name;

                        if (returnValue is IEnumerable returnValues)
                        {
                            var objectValueCount = returnValues.Cast<object>().Count();
                            typeName = $"{objectValueCount} {returnValue.GetType().GetGenericArguments()[0]?.Name}[]";
                            returnValue = string.Empty;
                        }
                        else
                        {
                            returnValue = $"<{returnValue}>";
                        }
                    
                        this._logger.LogInformation($"{DateTime.UtcNow.ToLongTimeString()}: Returned from {invocation.TargetType.Name} with {typeName} {returnValue}");
                    });
                }
                else
                {
                    this._logger.LogInformation($"{DateTime.UtcNow.ToLongTimeString()}: Returned from {invocation.TargetType.Name} with {typeName} {returnValue}");
                }
            }
            catch (Exception e)
            {
                this._logger.LogError(e, $"{DateTime.UtcNow.ToLongTimeString()}: !!! Logged Exception: {e.Message}");
                throw;
            }
        }

        private static string GetParameterDescription(object stringParameter, ParameterInfo parameterInfo)
        {
            var parameterValue = stringParameter?.ToString();

            if (!(stringParameter is string) && stringParameter is IEnumerable parameterValues)
            {
                parameterValue = string.Join(", ", parameterValues);
            }

            return $"<{parameterInfo.Name}: {parameterValue ?? string.Empty}>";
        }
    }
}