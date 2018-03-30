namespace AspNetCoreExt.Qos
{
    public interface IQosPolicyKeyComputer
    {
        string GetKey(QosPolicyKeyContext context);
    }
}
