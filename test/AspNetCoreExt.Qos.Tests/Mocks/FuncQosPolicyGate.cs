using System;
using System.Threading.Tasks;

namespace AspNetCoreExt.Qos.Tests.Mocks
{
    public class FuncQosPolicyGate : IQosPolicyGate
    {
        private readonly Func<QosGateEnterContext, QosGateEnterResult> _enterFunc;

        private readonly Action<QosGateExitContext> _exitAction;

        public FuncQosPolicyGate(
            Func<QosGateEnterContext, QosGateEnterResult> enterFunc,
            Action<QosGateExitContext> exitAction)
        {
            _enterFunc = enterFunc;
            _exitAction = exitAction;
        }

        public Task<QosGateEnterResult> TryEnterAsync(QosGateEnterContext context)
        {
            return Task.FromResult(_enterFunc(context));
        }

        public Task ExitAsync(QosGateExitContext context)
        {
            if (_exitAction != null)
            {
                _exitAction(context);
            }
            return Task.CompletedTask;
        }
    }
}
