using AspNetCoreExt.Qos.Abstractions.Infrastructure;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AspNetCoreExt.Qos.Tests.Mocks
{
    public class AlwaysVipMiddleware
    {
        private class AlwaysVipFeature : IVipFeature
        {
            public bool IsVip => true;
        }

        private readonly RequestDelegate _next;

        public AlwaysVipMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            context.Features.Set<IVipFeature>(new AlwaysVipFeature());
            return _next(context);
        }
    }
}
