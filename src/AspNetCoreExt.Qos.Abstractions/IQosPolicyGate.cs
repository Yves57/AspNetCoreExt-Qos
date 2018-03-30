using System.Threading.Tasks;

namespace AspNetCoreExt.Qos
{
    public interface IQosPolicyGate
    {
        Task<QosGateEnterResult> TryEnterAsync(QosGateEnterContext context);

        Task ExitAsync(QosGateExitContext context);
    }
}
