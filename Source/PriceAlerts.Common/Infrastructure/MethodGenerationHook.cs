using System;
using System.Reflection;
using Castle.DynamicProxy;

namespace PriceAlerts.Common.Infrastructure
{
    public class MethodGenerationHook : IProxyGenerationHook
    {
        public void MethodsInspected()
        {
        }

        public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo)
        {
        }

        public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
        {
            return !methodInfo.Name.StartsWith("set_") && !methodInfo.Name.StartsWith("get_");
        }
    }
}