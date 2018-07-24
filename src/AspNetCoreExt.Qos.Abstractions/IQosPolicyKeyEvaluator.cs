namespace AspNetCoreExt.Qos
{
    public interface IQosPolicyKeyEvaluator
    {
        string GetKey(QosPolicyKeyContext context);
    }
}
