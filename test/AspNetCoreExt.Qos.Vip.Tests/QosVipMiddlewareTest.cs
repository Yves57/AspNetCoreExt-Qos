using AspNetCoreExt.Qos.Abstractions.Infrastructure;
using AspNetCoreExt.Qos.Vip.Tests.Mocks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCoreExt.Qos.Vip.Tests
{
    public class QosVipMiddlewareTest
    {
        private static readonly string VipBodyContent = "VIP_BODY_CONTENT";

        private static readonly string NormalBodyContent = "BODY_CONTENT";

        [Fact]
        public async Task IpAddressIsVip()
        {
            var client = CreateClient(RemoteIpMiddleware.RemoteIpAddress);
            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, ""));
            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal(VipBodyContent, body);
        }

        [Fact]
        public async Task IpAddressIsNotVip()
        {
            var client = CreateClient("77.66.55.44");
            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, ""));
            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal(NormalBodyContent, body);
        }

        private HttpClient CreateClient(string vipAddress)
        {
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddQosVip(o => o.IpAddresses = new[] { vipAddress });
                })
                .Configure(app =>
                {
                    app.UseMiddleware<RemoteIpMiddleware>();
                    app.UseQosVip();
                    app.Run(context =>
                    {
                        if (context.Features.Get<IVipFeature>()?.IsVip ?? false)
                        {
                            return context.Response.WriteAsync(VipBodyContent);
                        }
                        return context.Response.WriteAsync(NormalBodyContent);
                    });
                });

            var server = new TestServer(builder);
            return server.CreateClient();
        }
    }
}
