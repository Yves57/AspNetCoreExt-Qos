namespace AspNetCoreExt.Qos.Internal
{
    public class SharedPolicyKeyComputerProvider : IQosPolicyKeyComputerProvider
    {
        public IQosPolicyKeyComputer TryCreate(object rawKeyComputer)
        {
            if (rawKeyComputer is string str && str == "*")
            {
                return new SharedPolicyKeyComputer();
            }

            return null;
        }
    }
}
