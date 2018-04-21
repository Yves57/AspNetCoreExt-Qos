using AspNetCoreExt.Qos;
using System.Threading.Tasks;

namespace QosSandbox.CustomPolicy
{
    public class MyCustomPolicyGate : IQosPolicyGate
    {
        public Task<QosGateEnterResult> TryEnterAsync(QosGateEnterContext context)
        {
            if (context.Key == "OK")
            {
                return Task.FromResult(new QosGateEnterResult()
                {
                    Success = true
                });
            }

            return Task.FromResult(new QosGateEnterResult()
            {
                Success = false
            });
        }

        public Task ExitAsync(QosGateExitContext context)
        {
            return Task.CompletedTask;
        }
    }
}
