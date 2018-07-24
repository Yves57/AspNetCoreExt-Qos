namespace AspNetCoreExt.Qos.Tests.Mocks
{
    class ConstantQosPolicyKeyEvaluator : IQosPolicyKeyEvaluator
    {
        private readonly string _key;

        public ConstantQosPolicyKeyEvaluator(string key)
        {
            _key = key;
        }

        public string GetKey(QosPolicyKeyContext context) => _key;
    }
}
