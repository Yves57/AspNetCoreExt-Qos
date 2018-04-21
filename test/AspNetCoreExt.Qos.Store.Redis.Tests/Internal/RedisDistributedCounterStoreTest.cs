using AspNetCoreExt.Qos.Store.Redis.Internal;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCoreExt.Qos.Store.Redis.Tests.Internal
{
    public class RedisDistributedCounterStoreTest
    {
        private const string SkipReason = "Disabled due to CI failure with Redis.";

        private static readonly QosRedisStoreOptions RedisOptions = new QosRedisStoreOptions()
        {
            Configuration = "localhost",
            InstanceName = "QosTest"
        };

        [Fact(Skip = SkipReason)]
        public async Task GetUnknownKey()
        {
            using (var store = CreateCounterStore())
            {
                var result = await store.GetAsync("GetUnknownKey");
                Assert.Equal(0, result);
            }
        }

        [Fact(Skip = SkipReason)]
        public async Task AddWithoutPeriod()
        {
            using (var store = CreateCounterStore())
            {
                var result = await store.AddAsync("AddWithoutPeriod", 10, null);
                Assert.Equal(10, result);

                await Task.Delay(2000);

                result = await store.AddAsync("AddWithoutPeriod", 5, null);
                Assert.Equal(15, result);
            }
        }

        [Fact(Skip = SkipReason)]
        public async Task AddWithPeriod()
        {
            using (var store = CreateCounterStore())
            {
                var result = await store.AddAsync("AddWithPeriod", 10, TimeSpan.FromSeconds(3));
                Assert.Equal(10, result);

                result = await store.GetAsync("AddWithPeriod");
                Assert.Equal(10, result);

                await Task.Delay(2000);

                result = await store.AddAsync("AddWithPeriod", 5, TimeSpan.FromSeconds(100)); // This TTL must be ignored because the key already exists
                Assert.Equal(15, result);

                await Task.Delay(2000); // Wait less than 10 seconds

                result = await store.AddAsync("AddWithPeriod", 5, TimeSpan.FromSeconds(3));
                Assert.Equal(5, result);
            }
        }

        [Fact(Skip = SkipReason)]
        public async Task TryAddWithoutPeriod()
        {
            using (var store = CreateCounterStore())
            {
                var result = await store.TryAddAsync("TryAddWithoutPeriod", 10, 20, null);
                Assert.True(result.Success);
                Assert.Equal(10, result.NewCounter);

                await Task.Delay(2000);

                result = await store.TryAddAsync("TryAddWithoutPeriod", 5, 20, null);
                Assert.True(result.Success);
                Assert.Equal(15, result.NewCounter);

                result = await store.TryAddAsync("TryAddWithoutPeriod", 5, 20, null);
                Assert.True(result.Success);
                Assert.Equal(20, result.NewCounter);

                result = await store.TryAddAsync("TryAddWithoutPeriod", 1, 20, null);
                Assert.False(result.Success);
                Assert.Equal(20, result.NewCounter);
            }
        }

        [Fact(Skip = SkipReason)]
        public async Task TryAddWithPeriod()
        {
            using (var store = CreateCounterStore())
            {
                var result = await store.TryAddAsync("TryAddWithPeriod", 10, 20, TimeSpan.FromSeconds(3));
                Assert.True(result.Success);
                Assert.Equal(10, result.NewCounter);

                result = await store.TryAddAsync("TryAddWithPeriod", 5, 20, TimeSpan.FromSeconds(100)); // This TTL must be ignored because the key already exists
                Assert.True(result.Success);
                Assert.Equal(15, result.NewCounter);

                await Task.Delay(2000);

                result = await store.TryAddAsync("TryAddWithPeriod", 5, 20, TimeSpan.FromSeconds(100)); // This TTL must be ignored because the key already exists
                Assert.True(result.Success);
                Assert.Equal(20, result.NewCounter);

                result = await store.TryAddAsync("TryAddWithPeriod", 1, 20, null);
                Assert.False(result.Success);
                Assert.Equal(20, result.NewCounter);

                await Task.Delay(2000);

                result = await store.TryAddAsync("TryAddWithPeriod", 3, 20, TimeSpan.FromSeconds(3));
                Assert.True(result.Success);
                Assert.Equal(3, result.NewCounter);
            }
        }

        [Fact(Skip = SkipReason)]
        public async Task RemoveUnknownKey()
        {
            using (var store = CreateCounterStore())
            {
                await store.RemoveAsync("RemoveUnknownKey");
            }
        }

        [Fact(Skip = SkipReason)]
        public async Task RemoveExistingKey()
        {
            using (var store = CreateCounterStore())
            {
                var result = await store.AddAsync("RemoveExistingKey", 10, null);
                Assert.Equal(10, result);

                await store.RemoveAsync("RemoveExistingKey");

                result = await store.GetAsync("RemoveExistingKey");
                Assert.Equal(0, result);
            }
        }

        private RedisDistributedCounterStore CreateCounterStore()
        {
            var store = new RedisDistributedCounterStore(Options.Create(RedisOptions));

            store.Database.Execute("FLUSHDB");

            return store;
        }
    }
}
