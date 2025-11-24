using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Neemle.XJson.Generated;
using Neemle.XJson.Tests.Models;
using Xunit;

namespace Neemle.XJson.Tests;

public class JsonEdgeCaseTests
{
    [Fact]
    public void Validate_ReturnsFalse_When_PropertyTypeUnsupported()
    {
        string json = "{\"notSupported\":1.23}";

        Json.Validate<HasUnsupported>(json, out var value, out var error).Should().BeFalse();
        value.Should().BeNull();
        error.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Decode_Throws_On_NonObject_And_Validate_Fails()
    {
        Action act = () => Json.Decode<Person>("[]");
        act.Should().Throw<JsonException>();

        Json.Validate<Person>("[]", out var value, out var error).Should().BeFalse();
        value.Should().BeNull();
        error.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Decode_Ignores_Unknown_Properties()
    {
        string json = "{\"street\":\"Main\",\"city\":\"NY\",\"extra\":123}";

        var addr = Json.Decode<Address>(json);
        addr.Street.Should().Be("Main");
        addr.City.Should().Be("NY");
    }

    [Fact]
    public void RoundTrip_Primitives_And_Nullables()
    {
        var model = new WithPrimitives
        {
            Active = true,
            Total = 1234567890123L,
            IsFlagged = false,
            OptionalCount = null
        };
        string json = Json.Encode(model);

        json.Should().Contain("\"active\"");
        json.Should().Contain("\"total\"");
        json.Should().Contain("\"isFlagged\"");
        json.Should().NotContain("\"optionalCount\""); // omitted because null and default WhenWritingNull

        var back = Json.Decode<WithPrimitives>(json);
        back.Active.Should().BeTrue();
        back.Total.Should().Be(1234567890123L);
        back.IsFlagged.Should().BeFalse();
        back.OptionalCount.Should().BeNull();
    }

    [Fact]
    public void ReferenceType_Null_Omitted_WhenWritingNull()
    {
        var model = new ParentWithChild { Home = null! };

        string json = Json.Encode(model); // default WhenWritingNull
        json.Should().NotContain("\"home\"");

        var back = Json.Decode<ParentWithChild>(json);
        back.Home.Should().BeNull();
    }

    [Fact]
    public void ReferenceType_Null_Written_When_Never()
    {
        var opts = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            WriteIndented = false
        };
        var model = new ParentWithChild { Home = null! };

        string json = Json.Encode(model, opts);
        json.Should().Contain("\"Home\":null");

        var back = Json.Decode<ParentWithChild>(json, opts);
        back.Home.Should().BeNull();
    }

    [Fact]
    public void ReferenceType_RoundTrips_When_Present()
    {
        var model = new ParentWithChild { Home = new Address { Street = "One", City = "Two" } };

        string json = Json.Encode(model);
        json.Should().Contain("\"home\"");
        json.Should().Contain("\"street\"");

        var back = Json.Decode<ParentWithChild>(json);
        back.Home.Should().NotBeNull();
        back.Home!.Street.Should().Be("One");
        back.Home.City.Should().Be("Two");
    }
}
