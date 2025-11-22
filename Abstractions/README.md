Neemle.XJson.Abstractions — attributes for XJson
================================================

This package contains the public attributes used by the XJson incremental source generator.
It is a tiny, runtime‑only dependency that you reference from the projects that define your
models. The generator itself lives in a separate package (`Neemle.XJson.Generator`).

What’s inside
-------------
- `Neemle.XJson.Abstractions.XJsonAttribute` — apply to classes or structs to opt‑in for
  reflection‑free JSON code generation.

Target frameworks: net8.0

Install
-------
Using NuGet packages (recommended):

```xml
<ItemGroup>
  <!-- Public attribute(s) used in your code -->
  <PackageReference Include="Neemle.XJson.Abstractions" Version="1.0.1" />

  <!-- Incremental source generator (build-time only) -->
  <PackageReference Include="Neemle.XJson.Generator"
                    Version="1.0.1"
                    PrivateAssets="all"
                    OutputItemType="Analyzer"
                    ReferenceOutputAssembly="false" />
</ItemGroup>
```

From source (local development):

```xml
<ItemGroup>
  <ProjectReference Include="..\Abstractions\Abstractions.csproj" />
  <ProjectReference Include="..\XJson.Generator\XJson.Generator.csproj"
                    OutputItemType="Analyzer"
                    ReferenceOutputAssembly="false" />
</ItemGroup>
```

Quick start
-----------
Annotate the models you want generated converters for with `[XJson]`.

```csharp
using System.Text.Json.Serialization;
using Neemle.XJson.Abstractions;

[XJson]
public class Person
{
    [JsonPropertyName("a")] // attribute name wins over any naming policy
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public int Age { get; set; }
}
```

Then, in your consuming code, use the generated helper from `Neemle.XJson.Generated` (see the
root XJson README for full API and options):

```csharp
using Neemle.XJson.Generated; // exposes static Json helper

var json = Json.Encode(new Person { Name = "Ada", Surname = "Lovelace", Age = 36 });
var copy = Json.Decode<Person>(json);
```

Notes
-----
- `[XJson]` can be placed on classes or structs.
- This package contains no runtime reflection and is trimming / NativeAOT friendly.
- For configuration, error‑tolerant decode, and more examples, see the root
  project README in the XJson repository.

License
-------
MIT — see the LICENSE file in the repository.
