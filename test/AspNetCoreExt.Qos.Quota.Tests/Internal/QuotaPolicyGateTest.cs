using AspNetCoreExt.Qos.Abstractions.Stores;
using AspNetCoreExt.Qos.Quota.Internal;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCoreExt.Qos.Quota.Tests.Internal
{
    public class QuotaPolicyGateTest
    {
        private long _counter = 0;

        private readonly IQosCounterStore _counterStore;

        public QuotaPolicyGateTest()
        {
            var store = new Mock<IQosCounterStore>(MockBehavior.Strict);
            store
                .Setup(s => s.GetAsync("k"))
                .ReturnsAsync<string, IQosCounterStore, long>(key =>
                {
                    return _counter;
                });
            store
                .Setup(s => s.AddAsync("k", It.IsAny<long>(), It.IsAny<TimeSpan?>()))
                .ReturnsAsync<string, long, TimeSpan?, IQosCounterStore, long>((key, increment, period) =>
                {
                    _counter += increment;
                    return _counter;
                });

            _counterStore = store.Object;
        }

        [Fact]
        public async Task EnterThenExit()
        {
            var writeCount = 200;
            var gate = new QuotaPolicyGate(_counterStore, TimeSpan.FromSeconds(1), 1000);
            var httpContext = new DefaultHttpContext();

            var enter = await gate.TryEnterAsync(new QosGateEnterContext()
            {
                HttpContext = httpContext,
                Key = "k"
            });

            Assert.True(enter.Success);
            Assert.Equal(0, _counter);

            await httpContext.Response.Body.WriteAsync(new byte[writeCount], 0, writeCount);

            await gate.ExitAsync(new QosGateExitContext()
            {
                HttpContext = httpContext,
                Key = "k"
            });

            Assert.Equal(writeCount, _counter);
        }

        [Fact]
        public async Task EnterUpToLimit()
        {
            var writeCount = 200;
            var gate = new QuotaPolicyGate(_counterStore, TimeSpan.FromSeconds(1), 300);
            var httpContext = new DefaultHttpContext();

            // First write
            var enter1 = await gate.TryEnterAsync(new QosGateEnterContext()
            {
                HttpContext = httpContext,
                Key = "k"
            });

            Assert.True(enter1.Success);
            Assert.Equal(0, _counter);

            await httpContext.Response.Body.WriteAsync(new byte[writeCount], 0, writeCount);

            await gate.ExitAsync(new QosGateExitContext()
            {
                HttpContext = httpContext,
                Key = "k",
                GateData = enter1.Data
            });

            Assert.Equal(writeCount, _counter);

            // Second write
            var enter2 = await gate.TryEnterAsync(new QosGateEnterContext()
            {
                HttpContext = httpContext,
                Key = "k"
            });

            Assert.True(enter2.Success);
            Assert.Equal(writeCount, _counter);

            await httpContext.Response.Body.WriteAsync(new byte[writeCount], 0, writeCount);

            await gate.ExitAsync(new QosGateExitContext()
            {
                HttpContext = httpContext,
                Key = "k",
                GateData = enter2.Data
            });

            Assert.Equal(writeCount * 2, _counter);

            // Third write
            var enter3 = await gate.TryEnterAsync(new QosGateEnterContext()
            {
                HttpContext = httpContext,
                Key = "k"
            });

            Assert.False(enter3.Success);
            Assert.Equal(writeCount * 2, _counter);
        }
    }
}
