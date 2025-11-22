using System.Text.Json.Serialization;
using Neemle.XJson.Abstractions;

namespace Neemle.XJson.Tests.Models;

[XJson]
public class Person
{
    [JsonPropertyName("a")] public string Name { get; set; } = string.Empty;
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

[XJson]
public class HasNullable
{
    public string? Optional { get; set; }
    public int? Count { get; set; }
}

[XJson]
public class HasUnsupported
{
    public double NotSupported { get; set; }
}

public class NotAnnotated
{
    public string Name { get; set; } = string.Empty;
}

[XJson]
public class WithWeirdNames
{
    public string HomeCity { get; set; } = string.Empty;
}
