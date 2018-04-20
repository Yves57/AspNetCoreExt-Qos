using AspNetCoreExt.Qos.ExpressionPolicyKeyComputer.Internal.Context;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AspNetCoreExt.Qos.ExpressionPolicyKeyComputer.Internal
{
    public class ExpressionPolicyKeyComputer : IQosPolicyKeyComputer
    {
        private static readonly string CompiledNamespace = "Compiled";

        private static readonly string CompiledClass = "CompiledKeyComputer";

        private static readonly string[] AllowedNamespaces = new string[]
            {
                "System", // DateTime, String,...
                "System.Linq" // Linq
            };

        private static readonly Assembly[] AllowedReferenceByTypes = new Assembly[]
            {
                typeof(object).Assembly,
                typeof(Enumerable).Assembly,
                typeof(IQosPolicyKeyComputer).Assembly,
                typeof(DefaultContext).Assembly
            };

        private static readonly string[] AllowedReferenceByNames = new string[]
            {
                "netstandard.dll",
                "System.Runtime.dll"
            };

        private readonly Func<DefaultContext, string> _compiledFunction;

        public ExpressionPolicyKeyComputer(string expression)
        {
            _compiledFunction = CompileFunction(expression);
        }

        public string GetKey(QosPolicyKeyContext context)
        {
            return _compiledFunction(new DefaultContext(
                context.HttpContext,
                context.RouteTemplate,
                context.RouteValues,
                DateTime.UtcNow));
        }

        private Func<DefaultContext, string> CompileFunction(string expression)
        {
            var sources = GetSourceCode(expression);
            var assembly = BuildAssembly(sources);

            var type = assembly.GetType($"{CompiledNamespace}.{CompiledClass}");
            var method = type.GetMethod(nameof(IQosPolicyKeyComputer.GetKey));

            return (Func<DefaultContext, string>)method.CreateDelegate(typeof(Func<DefaultContext, string>));
        }

        private List<string> GetSourceCode(string expression)
        {
            var lines = new List<string>();

            lines.AddRange(AllowedNamespaces.Select(s => $"using {s};"));
            lines.Add($"namespace {CompiledNamespace}");
            lines.Add("{");
            lines.Add($"   public static class {CompiledClass}");
            lines.Add("   {");
            lines.Add($"      public static string {nameof(IQosPolicyKeyComputer.GetKey)}({typeof(DefaultContext).FullName} context)");

            if (expression.TrimStart().StartsWith("{"))
            {
                // This a complete method source code
                lines.AddRange(expression.Split('\n'));
            }
            else
            {
                // This is just an expression
                lines.Add("      {");
                lines.Add($"         return {expression};");
                lines.Add("      }");
            }

            lines.Add("   }");
            lines.Add("}");

            return lines;
        }

        private Assembly BuildAssembly(List<string> sources)
        {
            var parseOptions = new CSharpParseOptions(LanguageVersion.CSharp7_2, DocumentationMode.None);
            var syntaxTree = CSharpSyntaxTree.ParseText(string.Join("\n", sources), parseOptions);

            var assemblyName = $"KeyProvider{Guid.NewGuid():N}";
            var compilationOptions = new CSharpCompilationOptions(
                outputKind: OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Release,
                deterministic: true);
            var compilation = CSharpCompilation.Create(assemblyName, new[] { syntaxTree }, GetReferences(), compilationOptions);

            using (var stream = new MemoryStream())
            {
                var result = compilation.Emit(stream);
                if (!result.Success)
                {
                    throw new Exception(string.Join("\n", result.Diagnostics));
                }

                return Assembly.Load(stream.ToArray());
            }
        }

        private IEnumerable<MetadataReference> GetReferences()
        {
            foreach (var refByType in AllowedReferenceByTypes)
            {
                yield return MetadataReference.CreateFromFile(refByType.Location);
            }

            var basePath = Path.GetDirectoryName(typeof(object).Assembly.Location);

            foreach (var refByName in AllowedReferenceByNames)
            {
                yield return MetadataReference.CreateFromFile(Path.Combine(basePath, refByName));
            }
        }
    }
}
