using AspNetCoreExt.Qos;
using AspNetCoreExt.Qos.Concurrency.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.AspNetCore.Builder
{
    public static class QosConcurrencyServicesExtensions
    {
        public static void AddQosConcurrency(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<IQosPolicyProvider, ConcurrencyPolicyProvider>();
        }
    }
}
