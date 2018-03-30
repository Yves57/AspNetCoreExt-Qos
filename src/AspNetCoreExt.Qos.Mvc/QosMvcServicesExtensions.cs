using AspNetCoreExt.Qos;
using AspNetCoreExt.Qos.Mvc.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.AspNetCore.Builder
{
    public static class QosMvcServicesExtensions
    {
        public static void AddQosMvc(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.Configure<MvcOptions>(o => o.Conventions.Add(new QosApplicationModelConvention()));
            services.AddSingleton<IQosPolicyPostConfigure, MvcQosPolicyPostConfigure>();
        }
    }
}
