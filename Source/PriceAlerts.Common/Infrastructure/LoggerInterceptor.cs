﻿using System;
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
                    description += GetParameterDescription(stringParameters[i], invocation.Method.GetParameters()[i]);
                    if (i < stringParameters.Length - 1)
                    {
                        description += ", ";
                    }
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
                    var objectValue = returnValues.Cast<object>().ToList();
                    typeName = $"{objectValue.Count} {returnValue.GetType().GetGenericArguments()[0]?.Name}[]";

                    returnValue = string.Join(", ", objectValue);
                }
            }

            this._logger.LogMessage($"{DateTime.UtcNow.ToLongTimeString()}: Returned from {invocation.TargetType.Name} with {typeName} <{returnValue}>");
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