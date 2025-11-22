XJson — reflection‑free JSON for System.Text.Json
=================================================

XJson is an incremental source generator that emits reflection‑free `System.Text.Json` converters for your models. It’s designed to be AOT/NativeAOT and trimming friendly: no runtime reflection, no `JsonSerializerContext` required, and no dynamic lookup of converters. You annotate your types, the generator produces per‑type converters, and a small static helper provides a simple API.

Features
--------
- Reflection‑free, AOT/NativeAOT friendly JSON serialization and deserialization
- Works under aggressive trimming (no runtime reflection or dynamic codegen)
- Simple helper API: `Json.Encode<T>`, `Json.Decode<T>`, and `Json.Validate<T>`
- Respects `[JsonPropertyName]` on properties (attribute name wins)
- Configurable via:
  - `JsonSerializerOptions` (runtime options)
  - `JsonSourceGenerationOptionsAttribute` (as a convenient options bag)
- CamelCase by default and “ignore nulls when writing”

Under the hood
---------------
XJson builds on System.Text.Json primitives — it generates code that uses `Utf8JsonWriter` and `Utf8JsonReader` directly. There is no reliance on `JsonSerializer.Serialize/Deserialize`, no `JsonSerializerContext`, and no runtime reflection. This keeps the pipeline small, fast, and friendly to trimming and NativeAOT.

What’s generated
----------------
For each type annotated with `[Neemle.XJson.Abstractions.XJson]`, the generator emits:
- An internal `JsonConverter<T>` with hand‑written (generated) `Utf8JsonWriter/Utf8JsonReader` logic
- A static helper in `Neemle.XJson.Generated` namespace:
  - `public static class Json`
    - `string Encode<T>(T value)`
    - `string Encode<T>(T value, JsonSerializerOptions? options)`
    - `string Encode<T>(T value, JsonSourceGenerationOptionsAttribute? genOptions)`
    - `T Decode<T>(string json)`
    - `T Decode<T>(string json, JsonSerializerOptions? options)`
    - `T Decode<T>(string json, JsonSourceGenerationOptionsAttribute? genOptions)`
    - `bool Validate<T>(string json, out T? value, out string? error)`
    - `bool Validate<T>(string json, out T? value, out string? error, JsonSerializerOptions? options)`
    - `bool Validate<T>(string json, out T? value, out string? error, JsonSourceGenerationOptionsAttribute? genOptions)`

Installation
------------
Add a project reference to the Abstractions and add the Generator as an analyzer to the project(s) where you define your models:

```xml
<ItemGroup>
  <ProjectReference Include="..\Abstractions\Abstractions.csproj" />
  <ProjectReference Include="..\XJson.Generator\XJson.Generator.csproj"
                    OutputItemType="Analyzer"
                    ReferenceOutputAssembly="false" />
</ItemGroup>

<!-- Optional but useful for debugging generator output -->
<PropertyGroup>
  <CompilerGeneratedFilesOutputPath>obj/Generated</CompilerGeneratedFilesOutputPath>
  <EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
  <Nullable>enable</Nullable>
  <ImplicitUsings>enable</ImplicitUsings>
</PropertyGroup>
```

Quick start
-----------
1) Annotate your models with `[XJson]` (and optionally standard STJ attributes like `[JsonPropertyName]`):

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
    public Address Address { get; set; } = new();
}

[XJson]
public class Address
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}
```

2) Serialize / deserialize using the generated helper:

```csharp
using System.Text.Json;
using Neemle.XJson.Generated; // exposes the static Json helper

var person = new Person { Name = "Ada", Surname = "Lovelace", Age = 36, Address = new Address { Street = "12 St James's Square", City = "London" } };

// Defaults: camelCase, ignore nulls, indented
string json = Json.Encode(person);
var copy = Json.Decode<Person>(json);

// Validate without throwing
if (Json.Validate<Person>(json, out var value, out var error))
{
    Console.WriteLine($"OK: {value!.Surname}");
}
else
{
    Console.WriteLine($"Invalid: {error}");
}
```

Configuring options
-------------------
You can pass either `JsonSerializerOptions` or `JsonSourceGenerationOptionsAttribute` to control naming and null handling.

- With `JsonSerializerOptions` (runtime options):

```csharp
var opts = new JsonSerializerOptions
{
    WriteIndented = false,
    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
};

string json = Json.Encode(person, opts);
var p = Json.Decode<Person>(json, opts);
```

- With `JsonSourceGenerationOptionsAttribute` (as an options bag):

```csharp
using System.Text.Json.Serialization;

var sg = new JsonSourceGenerationOptionsAttribute
{
    PropertyNamingPolicy = JsonKnownNamingPolicy.KebabCaseLower,
    WriteIndented = false,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
};

string json = Json.Encode(person, sg);
var p = Json.Decode<Person>(json, sg);
```

Property naming precedence
--------------------------
- `[JsonPropertyName("...")]` on a property always wins for both write and read.
- If no attribute is present, the configured naming policy is applied (e.g., camelCase by default).
- If neither attribute nor policy is provided, the CLR property name is used.

Supported naming policies from `JsonKnownNamingPolicy` map to `JsonNamingPolicy` when available on your target TFM:
- Always: `Unspecified` → `null`, `CamelCase` → `JsonNamingPolicy.CamelCase`
- .NET 8+: `SnakeCaseLower`, `SnakeCaseUpper`, `KebabCaseLower`, `KebabCaseUpper`
- .NET 9+: `TrainCase`

Supported types (v1)
--------------------
- Primitives: `string`, `bool`, `int`, `long` and their nullable variants
- Nested models: other `[XJson]` types (including nullable nested)

Current limitations (throw `NotSupportedException`)
---------------------------------------------------
- `double/float/decimal`, `Guid`, `DateTime/DateTimeOffset`
- Enums, arrays/lists/dictionaries, records, required members, polymorphism
- Custom per‑property converters

Null handling
-------------
- When `DefaultIgnoreCondition = WhenWritingNull`, reference and `Nullable<T>` properties with `null` are omitted on write.
- Otherwise they are emitted as `"prop": null`.

AOT and trimming notes
----------------------
- No `JsonSerializer.Serialize/Deserialize` calls are used in generated code paths.
- No `JsonSerializerOptions.GetConverter(Type)` or runtime reflection is used.
- Converters are invoked directly, making the approach compatible with trimming and NativeAOT.

Testing
-------
The repository includes a test project `XJson.Tests` (xUnit + FluentAssertions).

- How to run:
  - `dotnet test` (or run the `XJson.Tests` project directly). The tests target `net10.0` by default.

- What’s covered:
  - Encode/Decode round-trips for `[XJson]` types with nested objects
  - `[JsonPropertyName]` taking precedence over naming policies
  - Naming policies via both `JsonSerializerOptions` and `JsonSourceGenerationOptionsAttribute`
  - `Validate<T>` success/failure behavior with `error` messaging
  - Null handling differences between `WhenWritingNull` and `Never`
  - Errors for unsupported property types and for non‑annotated types

FAQ
---
Q: Do I still need `JsonSerializerContext` or STJ’s source generator?
A: No. XJson emits its own converters and a thin helper. You can still use STJ attributes like `[JsonPropertyName]` for naming.

Q: Can I mix XJson models with regular STJ serialization?
A: Yes, but XJson’s generated helper only handles `[XJson]` types. For others, use your regular STJ pipeline.

Q: How do I extend supported types?
A: Extend the generator to handle more primitives/collections/enums, or file an issue/PR.

License
-------
MIT — see LICENSE.
