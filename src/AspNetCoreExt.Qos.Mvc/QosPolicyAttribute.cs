using System;

namespace AspNetCoreExt.Qos.Mvc
{
    public class QosPolicyAttribute : Attribute
    {
        public QosPolicyAttribute(string policyName)
        {
            PolicyName = policyName ?? throw new ArgumentNullException(nameof(policyName));
        }

        public string PolicyName { get; }
    }
}
