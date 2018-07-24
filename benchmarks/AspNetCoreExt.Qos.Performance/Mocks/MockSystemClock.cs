using AspNetCoreExt.Qos.Internal;
using System;
using System.Collections.Generic;

namespace AspNetCoreExt.Qos.Performance.Mocks
{
    internal class MockSystemClock : ISystemClock
    {
        public DateTime UtcNow { get; set; } = new DateTime(2018, 7, 17);

        private List<ManualRunTimer> _timers = new List<ManualRunTimer>();

        public IDisposable AddTimer(Action callback, int period)
        {
            var timer = new ManualRunTimer(callback);
            _timers.Add(timer);
            return timer;
        }

        public void Run()
        {
            foreach (var timer in _timers)
            {
                timer.Run();
            }
        }

        private class ManualRunTimer : IDisposable
        {
            private readonly Action _callback;

            public ManualRunTimer(Action callback)
            {
                _callback = callback;
            }

            public void Run()
            {
                _callback();
            }

            public void Dispose()
            {
            }
        }

    }
}
