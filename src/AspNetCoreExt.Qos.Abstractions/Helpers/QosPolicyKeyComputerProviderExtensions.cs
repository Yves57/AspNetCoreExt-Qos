using System;
using System.Collections.Generic;

namespace AspNetCoreExt.Qos.Abstractions.Helpers
{
    public static class QosPolicyKeyComputerProviderExtensions
    {
        public static IQosPolicyKeyComputer Create(
            this IEnumerable<IQosPolicyKeyComputerProvider> keyComputerProviders,
            object rawKeyComputer)
        {
            foreach (var provider in keyComputerProviders)
            {
                var key = provider.TryCreate(rawKeyComputer);
                if (key != null)
                {
                    return key;
                }
            }

            throw new Exception($"Unable to find Key Computer Provider for {rawKeyComputer}.");
        }
    }
}
