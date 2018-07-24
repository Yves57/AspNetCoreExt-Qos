using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCoreExt.Qos.Mvc.Internal
{
    public class MvcQosPolicyPostConfigure : IQosPolicyPostConfigure
    {
        private readonly IApiDescriptionGroupCollectionProvider _apiProvider;

        public MvcQosPolicyPostConfigure(IApiDescriptionGroupCollectionProvider apiProvider)
        {
            _apiProvider = apiProvider;
        }

        public int Order => -1000;

        public void PostConfigure(IList<QosPolicy> policies)
        {
            foreach (var apiDescription in _apiProvider.ApiDescriptionGroups.Items.SelectMany(g => g.Items))
            {
                if (apiDescription.ActionDescriptor is ControllerActionDescriptor actionDescriptor)
                {
                    foreach (var policyAttribute in FindPolicyAttributes(actionDescriptor))
                    {
                        var policy = policies.FirstOrDefault(p => p.Name == policyAttribute.PolicyName);
                        if (policy == null)
                        {
                            throw new Exception($"No policy {policyAttribute.PolicyName} found for QoS MVC attribute.");
                        }

                        var urlTemplates = new List<QosUrlTemplate>();
                        if (policy.UrlTemplates != null)
                        {
                            urlTemplates.AddRange(policy.UrlTemplates);
                        }
                        urlTemplates.Add(new QosUrlTemplate(apiDescription.HttpMethod, actionDescriptor.AttributeRouteInfo.Template));

                        policy.UrlTemplates = urlTemplates;
                    }
                }
            }
        }

        private List<QosPolicyAttribute> FindPolicyAttributes(ControllerActionDescriptor actionDescriptor)
        {
            var policyAttributes = new List<QosPolicyAttribute>();

            policyAttributes.AddRange(actionDescriptor
                .MethodInfo
                .GetCustomAttributes(inherit: true)
                .OfType<QosPolicyAttribute>());


            var classType = actionDescriptor.MethodInfo.DeclaringType;
            do
            {
                policyAttributes.AddRange(classType
                    .GetCustomAttributes(inherit: false)
                    .OfType<QosPolicyAttribute>());

                classType = classType.BaseType;
            }
            while (classType != null);

            CheckUniquePolicies(policyAttributes);

            return policyAttributes;
        }

        private void CheckUniquePolicies(List<QosPolicyAttribute> policyAttributes)
        {
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var policy in policyAttributes)
            {
                if (names.Contains(policy.PolicyName))
                {
                    throw new Exception($"A same MVC action cannot have more than one policy attribute {policy.PolicyName}.");
                }

                names.Add(policy.PolicyName);
            }
        }
    }
}
