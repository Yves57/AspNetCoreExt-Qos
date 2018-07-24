using AspNetCoreExt.Qos;

namespace QosSandbox.CustomPolicy
{
    public class MyCustomPolicyKeyEvaluator : IQosPolicyKeyEvaluator
    {
        public string GetKey(QosPolicyKeyContext context)
        {
            if (context.HttpContext.Request.Headers.TryGetValue("MyKey", out var value)
                && !string.IsNullOrEmpty(value))
            {
                return value;
            }

            return null;
        }
    }
}
