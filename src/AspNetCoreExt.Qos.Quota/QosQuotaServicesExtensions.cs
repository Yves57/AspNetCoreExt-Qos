using AspNetCoreExt.Qos;
using AspNetCoreExt.Qos.Quota.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.AspNetCore.Builder
{
    public static class QosQuotaServicesExtensions
    {
        public static void AddQosQuota(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<IQosPolicyProvider, QuotaPolicyProvider>();
        }
    }
}
