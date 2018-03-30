using System.Collections.Generic;

namespace AspNetCoreExt.Qos.Internal
{
    public class DefaultQosPolicyPostConfigure : IQosPolicyPostConfigure
    {
        private readonly IQosRejectResponse _defaultRejectResponse;

        public DefaultQosPolicyPostConfigure(IQosRejectResponse defaultRejectResponse)
        {
            _defaultRejectResponse = defaultRejectResponse;
        }

        public int Order => 1000;

        public void PostConfigure(IList<QosPolicy> policies)
        {
            foreach (var policy in policies)
            {
                if (policy.RejectResponse == null)
                {
                    policy.RejectResponse = _defaultRejectResponse;
                }
            }
        }
    }
}
