using AspNetCoreExt.Qos.Abstractions.Stores;
using AspNetCoreExt.Qos.Concurrency.Internal;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCoreExt.Qos.Concurrency.Tests.Internal
{
    public class ConcurrentPolicyGateTest
    {
        private long _counter = 0;

        private readonly IQosCounterStore _counterStore;

        public ConcurrentPolicyGateTest()
        {
            var store = new Mock<IQosCounterStore>(MockBehavior.Strict);
            store
                .Setup(s => s.TryAddAsync("k", It.IsAny<long>(), It.IsAny<long>(), null))
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
            store
                .Setup(s => s.AddAsync("k", It.IsAny<long>(), null))
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
            var gate = new ConcurrentPolicyGate(_counterStore, 10);

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

            Assert.Equal(0, _counter);
        }

        [Fact]
        public async Task EnterUpToLimit()
        {
            var gate = new ConcurrentPolicyGate(_counterStore, 2);

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
