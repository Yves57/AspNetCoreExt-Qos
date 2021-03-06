﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace AspNetCoreExt.Qos.ExpressionPolicyKeyEvaluator.Internal.Context
{
    public class DefaultContext : IRequest, IRequestHeaders
    {
        private readonly HttpContext _httpContext;

        private readonly string _routeTemplate;

        private readonly IDictionary<string, string> _routeValues;

        public DefaultContext(
            HttpContext httpContext,
            RouteTemplate routeTemplate,
            RouteValueDictionary routeValues,
            DateTime timestamp)
        {
            _httpContext = httpContext;
            Timestamp = timestamp;
            _routeTemplate = routeTemplate?.TemplateText;
            _routeValues = routeValues?.ToDictionary(p => p.Key, p => p.Value?.ToString());
        }

        public IRequest Request => this;

        public DateTime Timestamp { get; }

        string IRequest.Method => _httpContext.Request.Method;

        X509Certificate2 IRequest.Certificate => _httpContext.Connection.ClientCertificate;

        IRequestHeaders IRequest.Headers => this;

        string IRequest.IpAddress => _httpContext.Connection.RemoteIpAddress.ToString();

        string IRequest.Url => $"{_httpContext.Request.Path}{_httpContext.Request.QueryString}";

        string IRequest.RouteTemplate => _routeTemplate;

        IDictionary<string, string> IRequest.RouteValues => _routeValues;

        ClaimsPrincipal IRequest.User => _httpContext.User;

        IEnumerable<string> IReadOnlyDictionary<string, string[]>.Keys => _httpContext.Request.Headers.Keys;

        IEnumerable<string[]> IReadOnlyDictionary<string, string[]>.Values => _httpContext.Request.Headers.Values.Select(s => s.ToArray());

        int IReadOnlyCollection<KeyValuePair<string, string[]>>.Count => _httpContext.Request.Headers.Count;

        string[] IReadOnlyDictionary<string, string[]>.this[string key]
        {
            get
            {
                if (_httpContext.Request.Headers.TryGetValue(key, out var values))
                {
                    return values.ToArray();
                }
                return null;
            }
        }

        bool IReadOnlyDictionary<string, string[]>.ContainsKey(string key) => _httpContext.Request.Headers.ContainsKey(key);

        IEnumerator<KeyValuePair<string, string[]>> IEnumerable<KeyValuePair<string, string[]>>.GetEnumerator() => new RequestHeaderEnumerator(_httpContext.Request.Headers.GetEnumerator());

        IEnumerator IEnumerable.GetEnumerator() => new RequestHeaderEnumerator(_httpContext.Request.Headers.GetEnumerator());

        bool IReadOnlyDictionary<string, string[]>.TryGetValue(string key, out string[] value)
        {
            if (_httpContext.Request.Headers.TryGetValue(key, out var values))
            {
                value = values.ToArray();
                return true;
            }

            value = null;
            return false;
        }

        string IRequestHeaders.GetValueOrDefault(string headerName, string defaultValue)
        {
            if (_httpContext.Request.Headers.TryGetValue(headerName, out var values))
            {
                if (values.Count == 0)
                {
                    return "";
                }
                if (values.Count == 1)
                {
                    return values[0];
                }
                return string.Join(",", (IEnumerable<string>)values);
            }
            return defaultValue;
        }
    }
}
