namespace AspNetCoreExt.Qos.Tests.Mocks
{
    class ConstantQosPolicyKeyComputer : IQosPolicyKeyComputer
    {
        private readonly string _key;

        public ConstantQosPolicyKeyComputer(string key)
        {
            _key = key;
        }

        public string GetKey(QosPolicyKeyContext context) => _key;
    }
}
