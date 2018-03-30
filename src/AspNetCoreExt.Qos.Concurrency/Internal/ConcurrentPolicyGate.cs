using AspNetCoreExt.Qos.Abstractions.Stores;
using System;
using System.Threading.Tasks;

namespace AspNetCoreExt.Qos.Concurrency.Internal
{
    public class ConcurrentPolicyGate : IQosPolicyGate
    {
        private readonly IQosCounterStore _store;

        private readonly int _maxCount;

        public ConcurrentPolicyGate(IQosCounterStore store, int maxCount)
        {
            if (maxCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxCount));
            }

            _store = store;
            _maxCount = maxCount;
        }

        public async Task<QosGateEnterResult> TryEnterAsync(QosGateEnterContext context)
        {
            var result = await _store.TryAddASync(context.Key, 1, _maxCount, null);

            return new QosGateEnterResult()
            {
                Success = result.Success
            };
        }

        public Task ExitAsync(QosGateExitContext context)
        {
            return _store.AddAsync(context.Key, -1);
        }
    }
}
