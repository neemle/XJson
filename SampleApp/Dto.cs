using System.Text.Json.Serialization;
using Neemle.XJson.Abstractions;

namespace Neemle.XJson.Sample;

[XJson]
public class Person
{
    [JsonPropertyName("a")]
    public string Name { get; set; } = string.Empty;
    
    public string Surname { get; set; } = string.Empty;

    [JsonIgnore]
    public string FullName => $"{Name} {Surname}";
    public int Age { get; set; }
    public Address Address { get; set; } = new();
}

[XJson]
public class Address
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}
