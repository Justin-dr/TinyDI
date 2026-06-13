using System;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace TinyDI
{
    [Generator]
    public class TinyGenerator : IIncrementalGenerator
    {
        private const string AttributeSource = "TinyDI.InjectAttribute.cs";
        private const string Source = "TinyDI.DI.cs";
        
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            IncrementalValueProvider<GeneratorOptions> optionsProvider =
                context.AnalyzerConfigOptionsProvider.Select(
                    static (options, _) =>
                    {
                        options.GlobalOptions.TryGetValue(
                            "build_property.TinyDINamespace",
                            out string? containerNamespace);

                        options.GlobalOptions.TryGetValue(
                            "build_property.TinyDIContainerName",
                            out string? containerName);
                        
                        options.GlobalOptions.TryGetValue(
                            "build_property.TinyDIAccessibility",
                            out string? configAccessibility);
                        
                        string accessibility = configAccessibility switch
                        {
                            "public" => "public",
                            _ => "internal"
                        };

                        return new GeneratorOptions(
                            containerNamespace ?? "TinyDI",
                            containerName ?? "DI",
                            accessibility);
                    });

            context.RegisterSourceOutput(
                optionsProvider,
                static (sourceContext, options) =>
                {
                    string source = GetSource(Source)
                        .Replace("__TINYDI_NAMESPACE__", options.Namespace)
                        .Replace("__TINYDI_CONTAINER__", options.ContainerName)
                        .Replace("internal", options.Accessbility);

                    sourceContext.AddSource(
                        $"{options.ContainerName}.g.cs",
                        source);

                    string attributeSource = GetSource(AttributeSource)
                        .Replace("__TINYDI_NAMESPACE__", options.Namespace)
                        .Replace("internal", options.Accessbility);
                    
                    sourceContext.AddSource(
                        "InjectAttribute.g.cs",
                        attributeSource);
                });
        }
        
        private static string GetSource(string source)
        {
            Assembly assembly = typeof(TinyGenerator).Assembly;

            using Stream stream = assembly.GetManifestResourceStream(source)
                                  ?? throw new InvalidOperationException($"Embedded resource '{source}' was not found.");

            using StreamReader reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }

        private readonly struct GeneratorOptions
        {
            public GeneratorOptions(string containerNamespace, string containerName, string accessibility)
            {
                Namespace = containerNamespace;
                ContainerName = containerName;
                Accessbility = accessibility;
            }

            public string Namespace { get; }

            public string ContainerName { get; }
            
            public string Accessbility { get; }
        }
    }
}