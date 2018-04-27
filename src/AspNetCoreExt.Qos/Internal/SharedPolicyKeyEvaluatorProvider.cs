namespace AspNetCoreExt.Qos.Internal
{
    public class SharedPolicyKeyEvaluatorProvider : IQosPolicyKeyEvaluatorProvider
    {
        public IQosPolicyKeyEvaluator TryCreate(object rawKeyEvaluator)
        {
            if (rawKeyEvaluator is string str && str == "*")
            {
                return new SharedPolicyKeyEvaluator();
            }

            return null;
        }
    }
}
