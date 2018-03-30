namespace AspNetCoreExt.Qos.Internal
{
    public class DirectQosPolicyKeyComputerProvider : IQosPolicyKeyComputerProvider
    {
        public IQosPolicyKeyComputer TryCreate(object rawKeyComputer)
        {
            if (rawKeyComputer is IQosPolicyKeyComputer key)
            {
                return key;
            }

            return null;
        }
    }
}
