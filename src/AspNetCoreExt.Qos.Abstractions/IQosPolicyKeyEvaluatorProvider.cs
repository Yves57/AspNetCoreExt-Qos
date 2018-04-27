namespace AspNetCoreExt.Qos
{
    public interface IQosPolicyKeyEvaluatorProvider
    {
        IQosPolicyKeyEvaluator TryCreate(object rawKeyEvaluator);
    }
}
