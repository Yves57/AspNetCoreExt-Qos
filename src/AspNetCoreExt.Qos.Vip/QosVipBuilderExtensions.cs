using AspNetCoreExt.Qos.Vip;
using System;

namespace Microsoft.AspNetCore.Builder
{
    public static class QosVipBuilderExtensions
    {
        public static IApplicationBuilder UseQosVip(this IApplicationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.UseMiddleware<QosVipMiddleware>();
        }
    }
}
