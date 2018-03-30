using AspNetCoreExt.Qos.Tests.Mocks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCoreExt.Qos.Tests
{
    public class QosMiddlewareTest
    {
        private static readonly string BodyContent = "BODY_CONTENT";

        private const string KeyConstant = "KEY";

        [Fact]
        public async Task Success()
        {
            var enterKeys = new string[] { "KEY0", null, "KEY2" };
            var enterData = new string[] { "DATA0", "DATA1", "DATA2" };
            var exitKeys = new string[3];
            var exitData = new string[3];
            var exitOrder = new List<string>();

            var client = CreateHttpClient(
                CreatePolicy(
                    "P0",
                    _ => new QosGateEnterResult() { Success = true, Data = enterData[0] },
                    context => { exitOrder.Add("P0"); exitKeys[0] = context.Key; exitData[0] = context.GateData as string; },
                    enterKeys[0]),
                CreatePolicy(
                    "P1",
                    _ => new QosGateEnterResult() { Success = true, Data = enterData[1] },
                    context => { exitOrder.Add("P1"); exitKeys[1] = context.Key; exitData[1] = context.GateData as string; },
                    enterKeys[1]),
                CreatePolicy(
                    "P2",
                    _ => new QosGateEnterResult() { Success = true, Data = enterData[2] },
                    context => { exitOrder.Add("P2"); exitKeys[2] = context.Key; exitData[2] = context.GateData as string; },
                    enterKeys[2]));

            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, ""));
            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(BodyContent, body);
            Assert.Equal(new string[] { "P2", "P0" }, exitOrder);
            Assert.Equal(enterKeys, exitKeys);
            Assert.Equal(new string[] { "DATA0", null, "DATA2" }, exitData);
        }

        [Fact]
        public async Task Reject()
        {
            var enterKeys = new string[] { "KEY0", "KEY1", "KEY2", "KEY3" };
            var enterData = new string[] { "DATA0", "DATA1", "DATA2", "DATA3" };
            var exitKeys = new string[4];
            var exitData = new string[4];
            var exitOrder = new List<string>();

            var client = CreateHttpClient(
                CreatePolicy(
                    "P0",
                    _ => new QosGateEnterResult() { Success = true, Data = enterData[0] },
                    context => { exitOrder.Add("P0"); exitKeys[0] = context.Key; exitData[0] = context.GateData as string; },
                    enterKeys[0]),
                CreatePolicy(
                    "P1",
                    _ => new QosGateEnterResult() { Success = true, Data = enterData[1] },
                    context => { exitOrder.Add("P1"); exitKeys[1] = context.Key; exitData[1] = context.GateData as string; },
                    enterKeys[1]),
                CreatePolicy(
                    "P2",
                    _ => new QosGateEnterResult() { Success = false, Data = enterData[2] },
                    context => { exitOrder.Add("P2"); exitKeys[2] = context.Key; exitData[2] = context.GateData as string; },
                    enterKeys[2]),
                CreatePolicy(
                    "P3",
                    _ => new QosGateEnterResult() { Success = true, Data = enterData[3] },
                    context => { exitOrder.Add("P3"); exitKeys[3] = context.Key; exitData[3] = context.GateData as string; },
                    enterKeys[3]));

            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, ""));
            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal(QosConstants.TooManyRequestHttpStatus, (int)response.StatusCode);
            Assert.Empty(body);
            Assert.Equal(new string[] { "P1", "P0" }, exitOrder);
            Assert.Equal(new string[] { "KEY0", "KEY1", null, null }, exitKeys);
            Assert.Equal(new string[] { "DATA0", "DATA1", null, null }, exitData);
        }

        [Fact]
        public async Task Vip()
        {
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddQos();
                    services.AddSingleton<IQosPolicyProvider>(
                        new ListQosPolicyProvider(
                            new[]
                            {
                                CreatePolicy(
                                    "P0",
                                    _ => new QosGateEnterResult() { Success = false },
                                    null)
                            }));
                })
                .Configure(app =>
                {
                    app.UseMiddleware<AlwaysVipMiddleware>();
                    app.UseQos();
                    app.Run(context =>
                    {
                        return context.Response.WriteAsync(BodyContent);
                    });
                });

            var server = new TestServer(builder);
            var client = server.CreateClient();

            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, ""));
            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(BodyContent, body);
        }



        private HttpClient CreateHttpClient(params QosPolicy[] policies)
        {
            var builder = new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddQos();
                    services.AddSingleton<IQosPolicyProvider>(new ListQosPolicyProvider(policies));
                })
                .Configure(app =>
                {
                    app.UseQos();
                    app.Run(context =>
                    {
                        return context.Response.WriteAsync(BodyContent);
                    });
                });

            var server = new TestServer(builder);
            return server.CreateClient();
        }

        private QosPolicy CreatePolicy(
            string name,
            Func<QosGateEnterContext, QosGateEnterResult> enterFunc,
            Action<QosGateExitContext> exitAction,
            string key = KeyConstant)
        {
            return new QosPolicy(name)
            {
                Order = 10,
                UrlTemplates = new[] { "*" },
                Gate = new FuncQosPolicyGate(enterFunc, exitAction),
                Key = new ConstantQosPolicyKeyComputer(key)
            };
        }
    }
}
