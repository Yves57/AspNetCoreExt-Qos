namespace AspNetCoreExt.Qos.Internal
{
    public class SharedPolicyKeyEvaluator : IQosPolicyKeyEvaluator
    {
        private static readonly string SharedPolicyPrefix = "QoS_Shared_";

        public string GetKey(QosPolicyKeyContext context)
        {
            return SharedPolicyPrefix + context.Policy.Name;
        }
    }
}
