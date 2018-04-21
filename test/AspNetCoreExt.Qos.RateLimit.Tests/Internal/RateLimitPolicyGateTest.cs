using AspNetCoreExt.Qos.Abstractions.Stores;
using AspNetCoreExt.Qos.RateLimit.Internal;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCoreExt.Qos.RateLimit.Tests.Internal
{
    public class RateLimitPolicyGateTest
    {
        private long _counter = 0;

        private readonly IQosCounterStore _counterStore;

        private readonly TimeSpan _period = TimeSpan.FromSeconds(1);

        public RateLimitPolicyGateTest()
        {
            var store = new Mock<IQosCounterStore>(MockBehavior.Strict);
            store
                .Setup(s => s.TryAddAsync("k", It.IsAny<long>(), It.IsAny<long>(), _period))
                .ReturnsAsync<string, long, long, TimeSpan?, IQosCounterStore, CounterStoreAddResult>((key, increment, maxCount, period) =>
                {
                    var success = false;
                    if (_counter + increment <= maxCount)
                    {
                        _counter += increment;
                        success = true;
                    }
                    return new CounterStoreAddResult()
                    {
                        Success = success,
                        NewCounter = _counter
                    };
                });

            _counterStore = store.Object;
        }

        [Fact]
        public async Task EnterThenExit()
        {
            var gate = new RateLimitPolicyGate(_counterStore, _period, 10);

            var enter = await gate.TryEnterAsync(new QosGateEnterContext()
            {
                Key = "k"
            });

            Assert.True(enter.Success);
            Assert.Equal(1, _counter);

            await gate.ExitAsync(new QosGateExitContext()
            {
                Key = "k"
            });

            Assert.Equal(1, _counter);
        }

        [Fact]
        public async Task EnterUpToLimit()
        {
            var gate = new RateLimitPolicyGate(_counterStore, _period, 2);

            var enter = await gate.TryEnterAsync(new QosGateEnterContext()
            {
                Key = "k"
            });

            Assert.True(enter.Success);
            Assert.Equal(1, _counter);

            enter = await gate.TryEnterAsync(new QosGateEnterContext()
            {
                Key = "k"
            });

            Assert.True(enter.Success);
            Assert.Equal(2, _counter);

            enter = await gate.TryEnterAsync(new QosGateEnterContext()
            {
                Key = "k"
            });

            Assert.False(enter.Success);
            Assert.Equal(2, _counter);
        }
    }
}
