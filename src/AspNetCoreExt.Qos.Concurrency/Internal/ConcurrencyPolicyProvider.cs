using AspNetCoreExt.Qos.Abstractions.Helpers;
using AspNetCoreExt.Qos.Abstractions.Stores;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCoreExt.Qos.Concurrency.Internal
{
    public class ConcurrencyPolicyProvider : IQosPolicyProvider
    {
        public QosPolicy[] _policies;

        public ConcurrencyPolicyProvider(
            IOptions<QosConcurrencyOptions> options,
            IEnumerable<IQosPolicyKeyComputerProvider> keyComputerProviders,
            IServiceProvider serviceProvider)
        {
            _policies = BuildPolicies(options.Value, keyComputerProviders, serviceProvider).ToArray();
        }

        public int Order => 1100;

        public IEnumerable<QosPolicy> GetPolicies() => _policies;

        private IEnumerable<QosPolicy> BuildPolicies(
            QosConcurrencyOptions options,
            IEnumerable<IQosPolicyKeyComputerProvider> keyComputerProviders,
            IServiceProvider serviceProvider)
        {
            if (options?.Policies != null)
            {
                foreach (var option in options.Policies)
                {
                    var policy = new QosPolicy(option.Key)
                    {
                        Order = -1300,
                        UrlTemplates = option.Value.UrlTemplates,
                        Key = keyComputerProviders.Create(option.Value.Key),
                        Gate = CreateGate(option.Value.MaxCount, option.Value.Distributed, serviceProvider)
                    };

                    yield return policy;
                }
            }
        }

        private IQosPolicyGate CreateGate(
            int maxCount,
            bool distributed,
            IServiceProvider serviceProvider)
        {
            var store = serviceProvider.GetService(distributed ? typeof(IQosDistributedCounterStore) : typeof(IQosMemoryCounterStore)) as IQosCounterStore;

            return new ConcurrentPolicyGate(store, maxCount);
        }
    }
}
