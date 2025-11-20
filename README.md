# XJson

XJson is a Roslyn incremental source generator that emits reflection-free `System.Text.Json` converters for classes or structs marked with `[XJson]`. It is designed for trimming and Native AOT compatibility, removing the usual reflection and linker config burdens while keeping the developer surface area minimal.

## Projects
- `Abstractions/`: Declares the `[XJson]` attribute that consumers apply to their DTOs.
- `XJson.Generator/`: The incremental generator that discovers marked types and produces strongly typed converters plus a helper surface.
- `SampleApp/`: Small console app demonstrating serialization and deserialization with the generated code.

## Installation
- Add the abstractions package to your project (NuGet ID `Neemle.XJson.Abstractions`).
- Add the generator package as an analyzer (NuGet ID `Neemle.XJson.Generator`). When adding via `dotnet`, mark it as an analyzer so the DLL lands under `analyzers/dotnet/cs`:
  ```xml
  <PackageReference Include="Neemle.XJson.Abstractions" Version="*" />
  <PackageReference Include="Neemle.XJson.Generator" Version="*" OutputItemType="Analyzer" ReferenceOutputAssembly="false" />
  ```

## Quick start
1) Mark your types:
```csharp
using Neemle.XJson.Abstractions;

[XJson]
public class Person
{
    public string Name { get; set; } = string.Empty;
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

2) Use the generated helpers:
```csharp
using Neemle.XJson.Generated;

var person = new Person
{
    Name = "Ada Lovelace",
    Age = 36,
    Address = new Address { Street = "12 St James's Square", City = "London" }
};

var json = XJsonGenerated.Serialize(person);
var roundTrip = XJsonGenerated.Deserialize<Person>(json);
```

## How it works
- The generator scans for public, non-abstract `[XJson]` types with readable/writable instance properties.
- It emits per-type converters and registers them in `Neemle.XJson.Generated.XJsonGenerated`.
- `XJsonGenerated.Serialize<T>()` and `.Deserialize<T>()` pick the right converter without reflection, enabling trimming/Native AOT without linker descriptors or RD.xml files.

## Building and running
- Build everything: `dotnet build xjson.slnx`
- Run the sample: `dotnet run --project SampleApp/SampleApp.csproj`

## Notes and limitations
- Handles primitive properties (string and integral numeric types) plus nested `[XJson]` types; other property types fall back to `JsonSerializer`.
- Properties must have both getter and setter.
- Native AOT and trimming work out of the box; there is no reflection use or linker configuration required.

## License
MIT – see `LICENSE`.
