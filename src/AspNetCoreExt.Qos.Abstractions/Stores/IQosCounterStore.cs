using System;
using System.Threading.Tasks;

namespace AspNetCoreExt.Qos.Abstractions.Stores
{
    public interface IQosCounterStore
    {
        Task<long> GetAsync(string key);

        Task<long> AddAsync(string key, long increment, TimeSpan? period);

        Task<CounterStoreAddResult> TryAddAsync(string key, long increment, long maxValue, TimeSpan? period);

        Task RemoveAsync(string key);

        Task ClearAsync();
    }

    public struct CounterStoreAddResult
    {
        public bool Success { get; set; }

        public long NewCounter { get; set; }
    }
}
