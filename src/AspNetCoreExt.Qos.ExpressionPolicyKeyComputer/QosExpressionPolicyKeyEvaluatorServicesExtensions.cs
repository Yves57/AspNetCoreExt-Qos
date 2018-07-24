using AspNetCoreExt.Qos;
using AspNetCoreExt.Qos.ExpressionPolicyKeyEvaluator.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.AspNetCore.Builder
{
    public static class QosExpressionPolicyKeyEvaluatorServicesExtensions
    {
        public static void AddQosExpressionPolicyKeyEvaluator(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<IQosPolicyKeyEvaluatorProvider, ExpressionPolicyKeyEvaluatorProvider>();
        }
    }
}
