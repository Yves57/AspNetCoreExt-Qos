using System.Collections.Generic;

namespace AspNetCoreExt.Qos.Tests.Mocks
{
    public class ListQosPolicyProvider : IQosPolicyProvider
    {
        public int Order => 0;

        private readonly IEnumerable<QosPolicy> _policies;

        public ListQosPolicyProvider(IEnumerable<QosPolicy> policies)
        {
            _policies = policies;
        }

        public IEnumerable<QosPolicy> GetPolicies() => _policies;
    }
}
