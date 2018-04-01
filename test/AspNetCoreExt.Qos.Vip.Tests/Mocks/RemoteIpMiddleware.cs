using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;

namespace AspNetCoreExt.Qos.Vip.Tests.Mocks
{
    public class RemoteIpMiddleware
    {
        public static readonly string RemoteIpAddress = "11.22.33.44";

        private readonly RequestDelegate _next;

        public RemoteIpMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            context.Connection.RemoteIpAddress = IPAddress.Parse(RemoteIpAddress);
            return _next(context);
        }
    }
}
