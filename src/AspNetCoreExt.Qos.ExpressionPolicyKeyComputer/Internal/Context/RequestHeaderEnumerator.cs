using Microsoft.Extensions.Primitives;
using System.Collections;
using System.Collections.Generic;

namespace AspNetCoreExt.Qos.ExpressionPolicyKeyEvaluator.Internal.Context
{
    public class RequestHeaderEnumerator : IEnumerator<KeyValuePair<string, string[]>>
    {
        private readonly IEnumerator<KeyValuePair<string, StringValues>> _baseEnumerator;

        public RequestHeaderEnumerator(IEnumerator<KeyValuePair<string, StringValues>> baseEnumerator)
        {
            _baseEnumerator = baseEnumerator;
        }

        public KeyValuePair<string, string[]> Current
        {
            get
            {
                var current = _baseEnumerator.Current;
                return new KeyValuePair<string, string[]>(current.Key, current.Value.ToArray());
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }

        public bool MoveNext() => _baseEnumerator.MoveNext();

        public void Reset()
        {
            _baseEnumerator.Reset();
        }
    }
}
