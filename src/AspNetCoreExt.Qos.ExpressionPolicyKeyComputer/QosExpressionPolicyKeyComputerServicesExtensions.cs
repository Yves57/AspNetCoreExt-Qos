using AspNetCoreExt.Qos;
using AspNetCoreExt.Qos.ExpressionPolicyKeyComputer.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.AspNetCore.Builder
{
    public static class QosExpressionPolicyKeyComputerServicesExtensions
    {
        public static void AddQosExpressionPolicyKeyComputer(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<IQosPolicyKeyComputerProvider, ExpressionPolicyKeyComputerProvider>();
        }
    }
}
