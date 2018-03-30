﻿using AspNetCoreExt.Qos.Abstractions.Helpers;
using AspNetCoreExt.Qos.Abstractions.Stores;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCoreExt.Qos.Quota.Internal
{
    public class QuotaPolicyProvider : IQosPolicyProvider
    {
        public QosPolicy[] _policies;

        public QuotaPolicyProvider(
            IOptions<QosQuotaOptions> options,
            IEnumerable<IQosPolicyKeyComputerProvider> keyComputerProviders,
            IServiceProvider serviceProvider)
        {
            _policies = BuildPolicies(options.Value, keyComputerProviders, serviceProvider).ToArray();
        }

        public int Order => 1400;

        public IEnumerable<QosPolicy> GetPolicies() => _policies;

        private IEnumerable<QosPolicy> BuildPolicies(
            QosQuotaOptions options,
            IEnumerable<IQosPolicyKeyComputerProvider> keyComputerProviders,
            IServiceProvider serviceProvider)
        {
            if (options?.Policies != null)
            {
                foreach (var option in options.Policies)
                {
                    var policy = new QosPolicy(option.Name)
                    {
                        Order = -1100,
                        UrlTemplates = option.UrlTemplates,
                        Key = keyComputerProviders.Create(option.Key),
                        Gate = CreateGate(option.Period, option.MaxCount, option.Distributed, serviceProvider)
                    };

                    yield return policy;
                }
            }
        }

        private IQosPolicyGate CreateGate(
            TimeSpan period,
            long maxCount,
            bool distributed,
            IServiceProvider serviceProvider)
        {
            var store = serviceProvider.GetService(distributed ? typeof(IQosDistributedCounterStore) : typeof(IQosMemoryCounterStore)) as IQosCounterStore;

            return new QuotaPolicyGate(store, period, maxCount);
        }
    }
}
