using System;
using System.Net.Http;

namespace AspNetCoreExt.Qos
{
    public class QosUrlTemplate
    {
        public string HttpMethod { get; }

        public string Url { get;  }

        public QosUrlTemplate(string httpMethod, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new FormatException($"URL template '{url}' cannot be empty.");
            }

            HttpMethod = httpMethod;
            Url = url;
        }

        public static QosUrlTemplate Parse(string urlTemplate)
        {
            if (string.IsNullOrEmpty(urlTemplate))
            {
                throw new FormatException($"URL template '{urlTemplate}' cannot be empty.");
            }

            var fields = urlTemplate.Split(' ');
            if (fields.Length == 1)
            {
                return new QosUrlTemplate(null, fields[0]);
            }
            if (fields.Length == 2)
            {
                if (fields[0].Length == 0)
                {
                    throw new FormatException($"Incorrect HTTP method name for '{urlTemplate}'.");
                }
                return new QosUrlTemplate(fields[0].ToUpperInvariant(), fields[1]);
            }

            throw new FormatException($"Incorrect syntax for URL template '{urlTemplate}'.");
        }

        public override string ToString()
        {
            return $"{HttpMethod ?? "*"} {Url}";
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            var other = obj as QosUrlTemplate;
            if (other == null)
            {
                return false;
            }
            return HttpMethod == other.HttpMethod && Url == other.Url;
        }
    }
}
