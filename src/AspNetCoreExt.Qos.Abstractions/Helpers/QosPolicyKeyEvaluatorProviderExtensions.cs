using System;
using System.Collections.Generic;

namespace AspNetCoreExt.Qos.Abstractions.Helpers
{
    public static class QosPolicyKeyEvaluatorProviderExtensions
    {
        public static IQosPolicyKeyEvaluator Create(
            this IEnumerable<IQosPolicyKeyEvaluatorProvider> keyEvaluatorProviders,
            object rawKeyEvaluator)
        {
            foreach (var provider in keyEvaluatorProviders)
            {
                var key = provider.TryCreate(rawKeyEvaluator);
                if (key != null)
                {
                    return key;
                }
            }

            throw new Exception($"Unable to find Key Evaluator Provider for {rawKeyEvaluator}.");
        }
    }
}
