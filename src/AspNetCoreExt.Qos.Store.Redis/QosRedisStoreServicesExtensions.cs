using AspNetCoreExt.Qos.Abstractions.Stores;
using AspNetCoreExt.Qos.Store.Redis;
using AspNetCoreExt.Qos.Store.Redis.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.AspNetCore.Builder
{
    public static class QosRedisStoreServicesExtensions
    {
        public static IServiceCollection AddQosRedisStore(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<IQosDistributedCounterStore, RedisDistributedCounterStore>();

            return services;
        }

        public static IServiceCollection AddQosRedisStore(this IServiceCollection services, Action<QosRedisStoreOptions> configureOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.AddSingleton<IQosDistributedCounterStore, RedisDistributedCounterStore>();
            services.Configure(configureOptions);

            return services;
        }
    }
}
