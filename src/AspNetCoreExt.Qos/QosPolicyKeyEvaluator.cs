using System;

namespace AspNetCoreExt.Qos
{
    public static class QosPolicyKeyEvaluator
    {
        public static IQosPolicyKeyEvaluator Create(Func<QosPolicyKeyContext, string> keyEvaluator)
        {
            if (keyEvaluator == null)
            {
                throw new ArgumentNullException(nameof(keyEvaluator));
            }

            return new FuncPolicyKeyEvaluator(keyEvaluator);
        }

        private class FuncPolicyKeyEvaluator : IQosPolicyKeyEvaluator
        {
            private readonly Func<QosPolicyKeyContext, string> _keyEvaluator;

            public FuncPolicyKeyEvaluator(Func<QosPolicyKeyContext, string> keyEvaluator)
            {
                _keyEvaluator = keyEvaluator;
            }

            public string GetKey(QosPolicyKeyContext context)
            {
                return _keyEvaluator(context);
            }
        }
    }
}
