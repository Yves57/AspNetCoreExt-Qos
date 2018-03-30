using System;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCoreExt.Qos.Internal
{
    public class PolicyBuilder
    {
        private readonly IEnumerable<IQosPolicyProvider> _policyProviders;

        private readonly IEnumerable<IQosPolicyPostConfigure> _policyPostConfigurations;

        public PolicyBuilder(
            IEnumerable<IQosPolicyProvider> policyProviders,
            IEnumerable<IQosPolicyPostConfigure> policyPostConfigurations)
        {
            _policyProviders = policyProviders;
            _policyPostConfigurations = policyPostConfigurations;
        }

        public IEnumerable<PolicyDescription> Build()
        {
            var policies = new List<QosPolicy>();

            foreach (var provider in _policyProviders.OrderBy(p => p.Order))
            {
                policies.AddRange(provider.GetPolicies());
            }

            CheckUniqueNames(policies);

            foreach (var postConfigure in _policyPostConfigurations.OrderBy(p => p.Order))
            {
                postConfigure.PostConfigure(policies);
            }

            return policies
                .OrderBy(p => p.Order)
                .Select(p => new PolicyDescription(p));
        }

        private void CheckUniqueNames(List<QosPolicy> policies)
        {
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var policy in policies)
            {
                if (names.Contains(policy.Name))
                {
                    throw new Exception($"Several policies have the name {policy.Name}.");
                }

                names.Add(policy.Name);
            }
        }
    }
}
