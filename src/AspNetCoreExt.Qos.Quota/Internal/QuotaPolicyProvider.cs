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
            IEnumerable<IQosPolicyKeyEvaluatorProvider> keyEvaluatorProviders,
            IServiceProvider serviceProvider)
        {
            _policies = BuildPolicies(options.Value, keyEvaluatorProviders, serviceProvider).ToArray();
        }

        public int Order => 1300;

        public IEnumerable<QosPolicy> GetPolicies() => _policies;

        private IEnumerable<QosPolicy> BuildPolicies(
            QosQuotaOptions options,
            IEnumerable<IQosPolicyKeyEvaluatorProvider> keyEvaluatorProviders,
            IServiceProvider serviceProvider)
        {
            if (options?.Policies != null)
            {
                foreach (var option in options.Policies)
                {
                    var policy = new QosPolicy(option.Key)
                    {
                        Order = -1100,
                        UrlTemplates = option.Value.UrlTemplates?.Select(u => QosUrlTemplate.Parse(u)),
                        Key = keyEvaluatorProviders.Create(option.Value.Key),
                        Gate = CreateGate(option.Value.Period, option.Value.MaxCount * 1024, option.Value.Distributed, serviceProvider)
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
