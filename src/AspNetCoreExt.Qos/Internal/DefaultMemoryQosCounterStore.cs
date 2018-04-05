using AspNetCoreExt.Qos.Abstractions.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreExt.Qos.Internal
{
    public class DefaultMemoryQosCounterStore : IQosMemoryCounterStore, IDisposable
    {
        private static readonly int OldEntriesCleanInterval = 100; // Same as Redis

        private static readonly int OldEntriesCleanSamplesPerIteration = 20; // Same as Redis

        private static readonly int OldEntriesCleanThreshold = 5; // Same as Redis

        private readonly Dictionary<string, StoreEntry> _counters = new Dictionary<string, StoreEntry>();

        private readonly ISystemClock _clock;

        private IDisposable _ttlTimer;

        private readonly Random _random = new Random(1234);

        public DefaultMemoryQosCounterStore(ISystemClock clock)
        {
            _clock = clock;
            _ttlTimer = _clock.AddTimer(CleanOldEntries, OldEntriesCleanInterval);
        }

        public void Dispose()
        {
            _ttlTimer?.Dispose();
        }

        public int Count
        {
            get
            {
                lock (_counters)
                {
                    return _counters.Count;
                }
            }
        }

        public Task<long> GetAsync(string key)
        {
            lock (_counters)
            {
                if (TryGet(key, out var counter))
                {
                    return Task.FromResult(counter.Counter);
                }
            }
            return Task.FromResult(0L);
        }

        public Task<long> AddAsync(string key, long increment, TimeSpan? period)
        {
            long result;

            lock (_counters)
            {
                if (TryGet(key, out var counter))
                {
                    counter.Counter += increment;
                    _counters[key] = counter;

                    result = counter.Counter;
                }
                else
                {
                    AddNewEntry(key, increment, period);
                    result = increment;
                }
            }

            return Task.FromResult(result);
        }

        public Task<CounterStoreAddResult> TryAddAsync(string key, long increment, long maxValue, TimeSpan? period)
        {
            long result;
            var success = true;

            lock (_counters)
            {
                if (TryGet(key, out var counter))
                {
                    if (counter.Counter + increment <= maxValue)
                    {
                        counter.Counter += increment;
                        _counters[key] = counter;
                    }
                    else
                    {
                        success = false;
                    }

                    result = counter.Counter;
                }
                else
                {
                    AddNewEntry(key, increment, period);

                    result = increment;
                }

            }

            return Task.FromResult(new CounterStoreAddResult()
            {
                Success = success,
                NewCounter = result
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

        private bool TryGet(string key, out StoreEntry counter)
        {
            if (_counters.TryGetValue(key, out counter))
            {
                if (counter.Ttl >= _clock.UtcNow)
                {
                    return true;
                }

                _counters.Remove(key);
            }

            return false;
        }

        private void AddNewEntry(string key, long counter, TimeSpan? period)
        {
            _counters.Add(key, new StoreEntry()
            {
                Counter = counter,
                Ttl = period.HasValue ? _clock.UtcNow + period.Value : DateTime.MaxValue
            });
        }

        private void CleanOldEntries()
        {
            // Same algorithm as Redis (https://redis.io/commands/expire)
            // Clearly absolutly horrible implementation, but it is a first one...
            lock (_counters)
            {
                int oldCounter;
                var keys = _counters.Keys.ToArray();
                var storeCount = keys.Length;
                var checkedKeys = new bool[storeCount];

                do
                {
                    oldCounter = 0;
                    var end = Math.Min(OldEntriesCleanSamplesPerIteration, storeCount);
                    for (int i = 0; i < end; i++)
                    {
                        for (int tries = 0; tries < 5; tries++) // Don't want to look for a key too long
                        {
                            var index = _random.Next(storeCount);
                            if (!checkedKeys[index])
                            {
                                checkedKeys[index] = true;
                                if (!TryGet(keys[index], out _))
                                {
                                    oldCounter++;
                                    storeCount--;
                                }
                                break;
                            }
                        }
                    }
                }
                while (oldCounter > OldEntriesCleanThreshold && storeCount > 0);
            }
        }

        private struct StoreEntry
        {
            public long Counter;

            public DateTime Ttl;
        }
    }
}
