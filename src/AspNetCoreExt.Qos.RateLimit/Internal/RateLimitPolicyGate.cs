using AspNetCoreExt.Qos.Abstractions.Stores;
using System;
using System.Threading.Tasks;

namespace AspNetCoreExt.Qos.RateLimit.Internal
{
    public class RateLimitPolicyGate : IQosPolicyGate
    {
        private readonly IQosCounterStore _store;

        private readonly TimeSpan _period;

        private readonly int _maxCount;

        public RateLimitPolicyGate(IQosCounterStore store, TimeSpan period, int maxCount)
        {
            if (period.Ticks <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(period));
            }
            if (maxCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxCount));
            }

            _store = store;
            _period = period;
            _maxCount = maxCount;
        }

        public async Task<QosGateEnterResult> TryEnterAsync(QosGateEnterContext context)
        {
            var result = await _store.TryAddAsync(context.Key, 1, _maxCount, _period);

            return new QosGateEnterResult()
            {
                Success = result.Success
            };
        }

        public Task ExitAsync(QosGateExitContext context)
        {
            return Task.CompletedTask;
        }
    }
}
