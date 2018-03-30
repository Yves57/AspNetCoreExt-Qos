using AspNetCoreExt.Qos;
using System;

namespace Microsoft.AspNetCore.Builder
{
    public static class QosBuilderExtensions
    {
        public static IApplicationBuilder UseQos(this IApplicationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.UseMiddleware<QosMiddleware>();
        }
    }
}
