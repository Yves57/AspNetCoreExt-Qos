using AspNetCoreExt.Qos;
using AspNetCoreExt.Qos.Abstractions.Stores;
using AspNetCoreExt.Qos.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.AspNetCore.Builder
{
    public static class QosServicesExtensions
    {
        public static void AddQos(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<PolicyBuilder>();
            services.TryAddSingleton<IQosRejectResponse, QosRejectResponse>();
            services.AddSingleton<IQosPolicyPostConfigure, DefaultQosPolicyPostConfigure>();
            services.AddSingleton<IQosPolicyKeyComputerProvider, DirectQosPolicyKeyComputerProvider>();
            services.AddSingleton<IQosPolicyKeyComputerProvider, SharedPolicyKeyComputerProvider>();
            services.AddSingleton<ISystemClock, DefaultSystemClock>();
            services.TryAddSingleton<IQosMemoryCounterStore, DefaultMemoryQosCounterStore>();
        }
    }
}
