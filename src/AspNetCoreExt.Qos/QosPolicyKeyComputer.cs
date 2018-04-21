using System;

namespace AspNetCoreExt.Qos
{
    public static class QosPolicyKeyComputer
    {
        public static IQosPolicyKeyComputer Create(Func<QosPolicyKeyContext, string> keyComputer)
        {
            if (keyComputer == null)
            {
                throw new ArgumentNullException(nameof(keyComputer));
            }

            return new FuncPolicyKeyComputer(keyComputer);
        }

        private class FuncPolicyKeyComputer : IQosPolicyKeyComputer
        {
            private readonly Func<QosPolicyKeyContext, string> _keyComputer;

            public FuncPolicyKeyComputer(Func<QosPolicyKeyContext, string> keyComputer)
            {
                _keyComputer = keyComputer;
            }

            public string GetKey(QosPolicyKeyContext context)
            {
                return _keyComputer(context);
            }
        }
    }
}
