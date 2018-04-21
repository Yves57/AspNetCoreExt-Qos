using AspNetCoreExt.Qos;
using System.Collections.Generic;

namespace QosSandbox.CustomPolicy
{
    public class MyCustomPolicyProvider : IQosPolicyProvider
    {
        public int Order => 0;

        public IEnumerable<QosPolicy> GetPolicies()
        {
            return new[]
            {
                new QosPolicy("MyCustomPolicyInstance")
                {
                    Order = 0,
                    Gate = new MyCustomPolicyGate(),
                    Key = new MyCustomPolicyKeyComputer(),
                    UrlTemplates = new[] { "*" }
                }
            };
        }
    }
}
