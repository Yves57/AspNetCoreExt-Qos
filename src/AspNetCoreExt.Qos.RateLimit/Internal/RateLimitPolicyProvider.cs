using AspNetCoreExt.Qos.Abstractions.Helpers;
using AspNetCoreExt.Qos.Abstractions.Stores;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCoreExt.Qos.RateLimit.Internal
{
    public class RateLimitPolicyProvider : IQosPolicyProvider
    {
        public QosPolicy[] _policies;

        public RateLimitPolicyProvider(
            IOptions<QosRateLimitOptions> options,
            IEnumerable<IQosPolicyKeyEvaluatorProvider> keyEvaluatorProviders,
            IServiceProvider serviceProvider)
        {
            _policies = BuildPolicies(options.Value, keyEvaluatorProviders, serviceProvider).ToArray();
        }

        public int Order => 1200;

        public IEnumerable<QosPolicy> GetPolicies() => _policies;

        private IEnumerable<QosPolicy> BuildPolicies(
            QosRateLimitOptions options,
            IEnumerable<IQosPolicyKeyEvaluatorProvider> keyEvaluatorProviders,
            IServiceProvider serviceProvider)
        {
            if (options?.Policies != null)
            {
                foreach (var option in options.Policies)
                {
                    var policy = new QosPolicy(option.Key)
                    {
                        Order = -1200,
                        UrlTemplates = option.Value.UrlTemplates?.Select(u => QosUrlTemplate.Parse(u)),
                        Key = keyEvaluatorProviders.Create(option.Value.Key),
                        Gate = CreateGate(option.Value.Period, option.Value.MaxCount, option.Value.Distributed, serviceProvider)
                    };

                    yield return policy;
                }
            }
        }

        private IQosPolicyGate CreateGate(
            TimeSpan period,
            int maxCount,
            bool distributed,
            IServiceProvider serviceProvider)
        {
            var store = serviceProvider.GetService(distributed ? typeof(IQosDistributedCounterStore) : typeof(IQosMemoryCounterStore)) as IQosCounterStore;

            return new RateLimitPolicyGate(store, period, maxCount);
        }
    }
}
