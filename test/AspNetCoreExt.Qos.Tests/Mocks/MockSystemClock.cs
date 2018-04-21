using AspNetCoreExt.Qos.Internal;
using System;
using System.Collections.Generic;

namespace AspNetCoreExt.Qos.Tests.Mocks
{
    public class MockSystemClock : ISystemClock
    {
        public static readonly DateTime DefaultUtcNow = new DateTime(2018, 4, 1);

        public List<Action> Callbacks { get; } = new List<Action>();

        public MockSystemClock()
        {
            UtcNow = DefaultUtcNow;
        }

        public DateTime UtcNow { get; set; }

        public IDisposable AddTimer(Action callback, int period)
        {
            Callbacks.Add(callback);
            return null;
        }
    }
}
