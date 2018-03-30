using AspNetCoreExt.Qos.Abstractions.Infrastructure;
using AspNetCoreExt.Qos.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreExt.Qos
{
    public class QosMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly PolicyDescription[] _policies;

        public QosMiddleware(
            RequestDelegate next,
            PolicyBuilder policyBuilder)
        {
            _next = next;

            _policies = policyBuilder.Build().ToArray(); // Use array for best performances
        }

        public async Task Invoke(HttpContext context)
        {
            if (IsVip(context))
            {
                await _next(context);
                return;
            }

            var execData = new ExecutionData[_policies.Length];

            var enterResult = await EnterAsync(context, execData);
            if (enterResult.RejectedPolicy != null)
            {
                await ExitAsync(context, execData, enterResult.RejectedPolicyIndex, true);

                var rejectResponseContext = new QosRejectResponseContext()
                {
                    HttpContext = context,
                    Policy = enterResult.RejectedPolicy
                };

                await enterResult.RejectedPolicy.RejectResponse.WriteAsync(rejectResponseContext);

                return;
            }

            try
            {
                await _next(context);
            }
            finally
            {
                await ExitAsync(context, execData, _policies.Length, false);
            }
        }

        private bool IsVip(HttpContext context)
        {
            var vipFeature = context.Features.Get<IVipFeature>();
            return vipFeature != null ? vipFeature.IsVip : false;
        }

        private async Task<(QosPolicy RejectedPolicy, int RejectedPolicyIndex)> EnterAsync(
            HttpContext context,
            ExecutionData[] execData)
        {
            var keyContext = new QosPolicyKeyContext(); // Perfs: allocate it only one time
            var enterContext = new QosGateEnterContext(); // Perfs: allocate it only one time

            for (var i = 0; i < _policies.Length; i++)
            {
                var policy = _policies[i];

                var routeValues = new RouteValueDictionary();
                RouteTemplate routeTemplate;
                if (policy.TryUrlMatching(context.Request.Path, routeValues, out routeTemplate))
                {
                    PrepareKeyContext(keyContext, context, routeTemplate, routeValues);

                    var key = policy.Policy.Key.GetKey(keyContext); // TODO Catch exceptions...
                    if (!string.IsNullOrEmpty(key))
                    {
                        PrepareGateEnterContext(enterContext, context, key);

                        var enterResult = await policy.Policy.Gate.TryEnterAsync(enterContext); // TODO Catch exceptions...

                        if (!enterResult.Success)
                        {
                            return (policy.Policy, i);
                        }

                        execData[i] = new ExecutionData()
                        {
                            Key = key,
                            GateData = enterResult.Data
                        };
                    }
                }
            }

            return (null, 0);
        }

        private void PrepareKeyContext(
            QosPolicyKeyContext context,
            HttpContext httpContext,
            RouteTemplate routeTemplate,
            RouteValueDictionary routeValues)
        {
            context.HttpContext = httpContext;
            context.RouteTemplate = routeTemplate;
            context.RouteValues = routeValues;
        }

        private void PrepareGateEnterContext(QosGateEnterContext context, HttpContext httpContext, string key)
        {
            context.HttpContext = httpContext;
            context.Key = key;
        }

        private async Task ExitAsync(HttpContext context, ExecutionData[] execData, int countToExit, bool rejectRequest)
        {
            var exitContext = new QosGateExitContext(); // Perfs: allocate it only one time

            for (var i = countToExit - 1; i >= 0; i--) // Leave in reverse direction
            {
                var key = execData[i].Key;
                if (key != null)
                {
                    try
                    {
                        exitContext.HttpContext = context;
                        exitContext.Key = key;
                        exitContext.GateData = execData[i].GateData;
                        exitContext.Reject = rejectRequest;

                        await _policies[i].Policy.Gate.ExitAsync(exitContext);
                    }
                    catch // Catch all exceptions to be able to execute the other releases
                    {
                        // TODO --> Log error?
                    }
                }
            }
        }

        private struct ExecutionData
        {
            public string Key { get; set; }

            public object GateData { get; set; }
        }
    }
}
