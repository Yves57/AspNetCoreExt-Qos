using AspNetCoreExt.Qos.Internal;
using AspNetCoreExt.Qos.Tests.Mocks;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCoreExt.Qos.Tests.Internal
{
    public class DefaultMemoryQosCounterStoreTest
    {
        [Fact]
        public async Task GetUnknownKey()
        {
            var clock = new MockSystemClock();
            var store = new DefaultMemoryQosCounterStore(clock);

            var result = await store.GetAsync("a");
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task AddAndGetExistingKey()
        {
            var clock = new MockSystemClock();
            var store = new DefaultMemoryQosCounterStore(clock);

            var result = await store.AddAsync("a", 3, null);
            Assert.Equal(3, result);

            result = await store.GetAsync("a");
            Assert.Equal(3, result);
        }

        [Fact]
        public async Task AddAndGetExistingButObsoleteKey()
        {
            var clock = new MockSystemClock();
            var store = new DefaultMemoryQosCounterStore(clock);

            var result = await store.AddAsync("a", 3, TimeSpan.FromSeconds(1));
            Assert.Equal(3, result);

            result = await store.GetAsync("a");
            Assert.Equal(3, result);

            clock.UtcNow += TimeSpan.FromSeconds(100);

            result = await store.GetAsync("a");
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task SuccessiveAdd()
        {
            var clock = new MockSystemClock();
            var store = new DefaultMemoryQosCounterStore(clock);

            var result = await store.AddAsync("a", 3, TimeSpan.FromSeconds(1));
            Assert.Equal(3, result);

            result = await store.AddAsync("a", 2, TimeSpan.FromSeconds(500)); // The period must be ignored
            Assert.Equal(5, result);

            clock.UtcNow += TimeSpan.FromSeconds(100);

            result = await store.AddAsync("a", 4, TimeSpan.FromSeconds(1));
            Assert.Equal(4, result);
        }

        [Fact]
        public async Task SuccessiveTryAdd()
        {
            var clock = new MockSystemClock();
            var store = new DefaultMemoryQosCounterStore(clock);

            var result = await store.TryAddAsync("a", 3, 6, TimeSpan.FromSeconds(1));
            Assert.True(result.Success);
            Assert.Equal(3, result.NewCounter);

            result = await store.TryAddAsync("a", 2, 6, TimeSpan.FromSeconds(500)); // The period must be ignored
            Assert.True(result.Success);
            Assert.Equal(5, result.NewCounter);

            result = await store.TryAddAsync("a", 1, 6, TimeSpan.FromSeconds(500));
            Assert.True(result.Success);
            Assert.Equal(6, result.NewCounter);

            result = await store.TryAddAsync("a", 1, 6, TimeSpan.FromSeconds(500));
            Assert.False(result.Success);
            Assert.Equal(6, result.NewCounter);

            clock.UtcNow += TimeSpan.FromSeconds(100);

            result = await store.TryAddAsync("a", 3, 6, TimeSpan.FromSeconds(1));
            Assert.True(result.Success);
            Assert.Equal(3, result.NewCounter);
        }

        [Fact]
        public async Task Remove()
        {
            var clock = new MockSystemClock();
            var store = new DefaultMemoryQosCounterStore(clock);

            await store.AddAsync("a", 3, TimeSpan.FromSeconds(1));
            await store.AddAsync("b", 4, TimeSpan.FromSeconds(1));

            await store.RemoveAsync("b");

            Assert.Equal(1, store.Count);
        }

        [Fact]
        public async Task CleanObsoleteEntries()
        {
            var clock = new MockSystemClock();
            var store = new DefaultMemoryQosCounterStore(clock);

            for (int i = 0; i < 30; i++)
            {
                await store.AddAsync(i.ToString(), 3, TimeSpan.FromSeconds(i));
            }

            Assert.Equal(30, store.Count);

            clock.Callbacks.First().Invoke();

            Assert.Equal(30, store.Count);

            clock.UtcNow += TimeSpan.FromSeconds(14);
            clock.Callbacks.First().Invoke();

            Assert.Equal(16, store.Count);

            clock.UtcNow += TimeSpan.FromSeconds(5);
            clock.Callbacks.First().Invoke();

            Assert.Equal(11, store.Count);
        }
    }
}
