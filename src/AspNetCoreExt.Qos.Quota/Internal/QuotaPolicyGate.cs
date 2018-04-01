using AspNetCoreExt.Qos.Abstractions.Stores;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AspNetCoreExt.Qos.Quota.Internal
{
    public class QuotaPolicyGate : IQosPolicyGate
    {
        private readonly IQosCounterStore _store;

        private readonly TimeSpan _period;

        private readonly long _maxCount;

        public QuotaPolicyGate(IQosCounterStore store, TimeSpan period, long maxCount)
        {
            if (_period.Ticks <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(period));
            }

            _store = store;
            _period = period;
            _maxCount = maxCount;
        }

        public async Task<QosGateEnterResult> TryEnterAsync(QosGateEnterContext context)
        {
            var count = await _store.GetAsync(context.Key);
            if (count > _maxCount)
            {
                return new QosGateEnterResult()
                {
                    Success = false
                };
            }

            var originalBodyStream = context.HttpContext.Response.Body;
            var originalSendFileFeature = context.HttpContext.Features.Get<IHttpSendFileFeature>();

            var wrapperStream = new BodyResponseStream(originalBodyStream, originalSendFileFeature);

            context.HttpContext.Response.Body = wrapperStream;
            if (originalSendFileFeature != null)
            {
                context.HttpContext.Features.Set<IHttpSendFileFeature>(wrapperStream);
            }

            return new QosGateEnterResult()
            {
                Success = true,
                Data = originalBodyStream
            };
        }

        public Task ExitAsync(QosGateExitContext context)
        {
            var wrapperStream = (BodyResponseStream)context.HttpContext.Response.Body;

            context.HttpContext.Response.Body = (Stream)context.GateData;
            if (wrapperStream.InnerSendFileFeature != null)
            {
                context.HttpContext.Features.Set(wrapperStream.InnerSendFileFeature);
            }

            return _store.AddAsync(context.Key, wrapperStream.WrittenLength, _period);
        }
    }
}
