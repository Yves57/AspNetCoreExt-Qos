namespace AspNetCoreExt.Qos.Internal
{
    public class DirectQosPolicyKeyEvaluatorProvider : IQosPolicyKeyEvaluatorProvider
    {
        public IQosPolicyKeyEvaluator TryCreate(object rawKeyEvaluator)
        {
            if (rawKeyEvaluator is IQosPolicyKeyEvaluator key)
            {
                return key;
            }

            return null;
        }
    }
}
