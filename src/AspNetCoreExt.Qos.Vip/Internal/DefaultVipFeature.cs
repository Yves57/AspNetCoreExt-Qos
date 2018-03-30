using AspNetCoreExt.Qos.Abstractions.Infrastructure;

namespace AspNetCoreExt.Qos.Vip.Internal
{
    public class DefaultVipFeature : IVipFeature
    {
        public bool IsVip => true;
    }
}
