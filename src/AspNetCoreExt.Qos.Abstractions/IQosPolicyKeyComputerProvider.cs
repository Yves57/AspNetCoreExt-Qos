namespace AspNetCoreExt.Qos
{
    public interface IQosPolicyKeyComputerProvider
    {
        IQosPolicyKeyComputer TryCreate(object rawKeyComputer);
    }
}
