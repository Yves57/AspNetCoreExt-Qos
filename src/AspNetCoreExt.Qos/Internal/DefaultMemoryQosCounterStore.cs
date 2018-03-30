using AspNetCoreExt.Qos.Abstractions.Stores;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCoreExt.Qos.Concurrency.Internal
{
    public class DefaultMemoryQosCounterStore : IQosMemoryCounterStore
    {
        private readonly Dictionary<string, long> _counters = new Dictionary<string, long>();

        public Task<long> GetAsync(string key)
        {
            var counter = 0L;

            lock (_counters)
            {
                _counters.TryGetValue(key, out counter);
            }

            return Task.FromResult(counter);
       }

        public Task<long> AddAsync(string key, long increment)
        {
            var counter = 0L;

            lock (_counters)
            {
                _counters.TryGetValue(key, out counter);
                counter += increment;
                _counters[key] = counter;
            }

            return Task.FromResult(counter);
        }

        public Task<CounterStoreAddResult> TryAddASync(string key, long increment, long maxValue, TimeSpan? period)
        {
            //TODO --> Gérer la période
            var counter = 0L;
            var success = true;

            lock (_counters)
            {
                _counters.TryGetValue(key, out counter);

                if (counter + increment <= maxValue)
                {
                    counter += increment;
                    _counters[key] = counter;
                }
                else
                {
                    success = false;
                }
            }

            return Task.FromResult(new CounterStoreAddResult()
            {
                Success = success,
                NewCounter = counter
            });
        }

        public Task RemoveAsync(string key)
        {
            lock (_counters)
            {
                _counters.Remove(key);
            }
            return Task.CompletedTask;
        }

        public Task ClearAsync()
        {
            lock (_counters)
            {
                _counters.Clear();
            }
            return Task.CompletedTask;
        }
    }
}
