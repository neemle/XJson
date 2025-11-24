using System.Collections.Immutable;
using System.Reflection;
using System.Text.Json;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Neemle.XJson.Generator;
using Xunit;

namespace Neemle.XJson.Generator.Tests;

public class GeneratorTests
{
    [Fact]
    public void GeneratesConverters_ForAnnotatedTypes()
    {
        var sources = @"
using Neemle.XJson.Abstractions;
namespace Demo;
[XJson] public class Address { public string Street { get; set; } = string.Empty; }
[XJson] public class Person { public string Name { get; set; } = string.Empty; public Address Home { get; set; } = new(); }
";

        var generated = RunGenerator(sources);

        generated.Should().Contain("internal sealed class Demo_Address_Converter");
        generated.Should().Contain("internal sealed class Demo_Person_Converter");
        generated.Should().Contain("public static class Json");
    }

    [Fact]
    public void EmitsNotSupported_ForNullableReferenceTypes()
    {
        var sources = @"
using Neemle.XJson.Abstractions;
namespace Demo;
[XJson] public class Address { public string Street { get; set; } = string.Empty; }
[XJson] public class Parent { public Address? Home { get; set; } }
";

        var generated = RunGenerator(sources);

        generated.Should().Contain("Property type not supported: global::Demo.Address?");
    }

    [Fact]
    public void EmitsClearNotSupported_ForUnsupportedProperty()
    {
        var sources = @"
using Neemle.XJson.Abstractions;
namespace Demo;
[XJson] public class Unsupported { public double Value { get; set; } }
";

        var generated = RunGenerator(sources);

        generated.Should().Contain("Property type not supported: double");
    }

    private static string RunGenerator(string source)
    {
        const string attr = @"
namespace Neemle.XJson.Abstractions
{
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
    public sealed class XJsonAttribute : System.Attribute { }
}";

        var parseOptions = new CSharpParseOptions(LanguageVersion.Latest);
        var syntaxTrees = new[]
        {
            CSharpSyntaxTree.ParseText(attr, parseOptions),
            CSharpSyntaxTree.ParseText(source, parseOptions)
        };

        var refs = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Enumerable).GetTypeInfo().Assembly.Location),
            MetadataReference.CreateFromFile(typeof(JsonSerializerOptions).GetTypeInfo().Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Text.Json.Serialization.JsonPropertyNameAttribute).GetTypeInfo().Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location),
        };

        var compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: syntaxTrees,
            references: refs,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithNullableContextOptions(NullableContextOptions.Enable));

        var compDiags = compilation.GetDiagnostics();
        compDiags.Should().BeEmpty("input compilation should be error-free");

        var generator = new XJsonSourceGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            new ISourceGenerator[] { generator.AsSourceGenerator() },
            parseOptions: parseOptions);

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out _, out var diagnostics);
        diagnostics.Should().BeEmpty("generator should not emit diagnostics");

        var runResult = driver.GetRunResult();
        var generated = string.Join("\n\n", runResult.Results.SelectMany(r => r.GeneratedSources).Select(s => s.SourceText.ToString()));
        generated.Should().NotBeNullOrWhiteSpace("generator should emit source");

        return generated;
    }
}
