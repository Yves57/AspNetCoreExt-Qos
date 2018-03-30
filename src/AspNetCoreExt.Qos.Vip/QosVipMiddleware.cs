using AspNetCoreExt.Qos.Abstractions.Infrastructure;
using AspNetCoreExt.Qos.Vip.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AspNetCoreExt.Qos.Vip
{
    public class QosVipMiddleware
    {
        private static readonly IVipFeature _defaultVipFeature = new DefaultVipFeature();

        private readonly RequestDelegate _next;

        private IPAddress[] _ipAddresses;

        public QosVipMiddleware(
            RequestDelegate next,
            IOptions<QosVipOptions> options)
        {
            _next = next;

            var opt = options.Value;

            if (opt.IpAddresses != null)
            {
                _ipAddresses = opt
                    .IpAddresses
                    .Select(a => IPAddress.Parse(a))
                    .ToArray(); // Array for performance
            }
            else
            {
                _ipAddresses = new IPAddress[0];
            }
        }

        public Task Invoke(HttpContext context)
        {
            if (_ipAddresses.Contains(context.Connection.RemoteIpAddress))
            {
                context.Features.Set(_defaultVipFeature);
            }

            return _next(context);
        }
    }
}
