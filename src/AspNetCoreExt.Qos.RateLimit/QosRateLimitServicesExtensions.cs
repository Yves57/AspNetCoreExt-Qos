using AspNetCoreExt.Qos;
using AspNetCoreExt.Qos.RateLimit.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.AspNetCore.Builder
{
    public static class QosRateLimitServicesExtensions
    {
        public static void AddQosRateLimit(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<IQosPolicyProvider, RateLimitPolicyProvider>();
        }
    }
}
