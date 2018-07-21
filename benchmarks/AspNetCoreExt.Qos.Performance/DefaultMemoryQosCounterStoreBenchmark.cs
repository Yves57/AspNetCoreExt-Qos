using AspNetCoreExt.Qos.Internal;
using AspNetCoreExt.Qos.Performance.Mocks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using System;

namespace AspNetCoreExt.Qos.Performance
{
    [ShortRunJob]
    public class DefaultMemoryQosCounterStoreBenchmark
    {
        private const int CleanCount = 5;

        [Params(1000, 100_000, 10_000_000)]
        public int EntryCount { get; set; }

        [Params(0.3, 0.8)]
        public double TimeoutPercentage { get; set; }

        private DefaultMemoryQosCounterStore _store;

        private MockSystemClock _systemClock;

        [IterationSetup]
        public void Setup()
        {
            _systemClock = new MockSystemClock();
            _store = new DefaultMemoryQosCounterStore(_systemClock);

            var random = new Random(123);
            for (var i = 0; i < EntryCount; i++)
            {
                var inTimeout = random.NextDouble() < TimeoutPercentage;
                _store.AddAsync($"Key_{i}", 1, inTimeout ? TimeSpan.FromSeconds(1) : TimeSpan.FromDays(1000)).Wait();
            }

            _systemClock.UtcNow += TimeSpan.FromHours(1);
        }

        [Benchmark(OperationsPerInvoke = CleanCount)]
        public void CleanOldEntries()
        {
            for (int i = 0; i < CleanCount; i++)
            {
                _systemClock.Run();
            }
        }
    }
}

