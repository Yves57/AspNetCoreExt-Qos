using AspNetCoreExt.Qos.Abstractions.Helpers;
using System;

namespace AspNetCoreExt.Qos.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class QosPolicyAttribute : Attribute
    {
        public QosPolicyAttribute(string policyName)
        {
            if (!PolicyNameHelper.IsValid(policyName))
            {
                throw new ArgumentException("Policy name cannot be empty.", nameof(policyName));
            }

            PolicyName = policyName;
        }

        public string PolicyName { get; }
    }
}
