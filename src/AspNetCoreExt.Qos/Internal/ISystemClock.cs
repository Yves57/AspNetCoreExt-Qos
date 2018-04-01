using System;

namespace AspNetCoreExt.Qos.Internal
{
    public interface ISystemClock
    {
        DateTime UtcNow { get; }

        IDisposable AddTimer(Action callback, int period);
    }
}
