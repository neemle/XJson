using System.Text.Json;
using FluentAssertions;
using Neemle.XJson.Generated;
using Neemle.XJson.Tests.Models;
using Xunit;

namespace Neemle.XJson.Tests;

public class JsonEncodeDecodeTests
{
    [Fact]
    public void RoundTrip_DefaultOptions_Works()
    {
        var p = new Person
        {
            Name = "Ada",
            Surname = "Lovelace",
            Age = 36,
            Address = new Address { Street = "12 St James's Square", City = "London" }
        };

        string json = Json.Encode(p);
        json.Should().Contain("\"a\""); // JsonPropertyName override for Name
        json.Should().Contain("\"age\"");
        json.Should().Contain("\"address\"");

        var copy = Json.Decode<Person>(json);
        copy.Name.Should().Be("Ada");
        copy.Surname.Should().Be("Lovelace");
        copy.Age.Should().Be(36);
        copy.Address.City.Should().Be("London");
        copy.Address.Street.Should().Contain("St James");
    }

    [Fact]
    public void AttributeName_WinsOverNamingPolicy()
    {
        var p = new Person { Name = "A", Surname = "B", Age = 1, Address = new Address { Street = "x", City = "y" } };
        var opts = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower, WriteIndented = false };

        string json = Json.Encode(p, opts);

        json.Should().Contain("\"a\""); // attribute wins
        json.Should().Contain("\"surname\""); // policy applies to others
        json.Should().NotContain("\"name\"");

        var copy = Json.Decode<Person>(json, opts);
        copy.Name.Should().Be("A");
        copy.Surname.Should().Be("B");
    }
}
