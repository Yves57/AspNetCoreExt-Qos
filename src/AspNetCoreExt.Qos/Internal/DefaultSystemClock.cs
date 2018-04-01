using System;
using System.Threading;

namespace AspNetCoreExt.Qos.Internal
{
    public class DefaultSystemClock : ISystemClock
    {
        public DateTime UtcNow => DateTime.UtcNow;

        public IDisposable AddTimer(Action callback, int period)
        {
            return new Timer(_ => callback(), null, period, period);
        }
    }
}
