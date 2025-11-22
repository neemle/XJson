using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Neemle.XJson.Generated;
using Neemle.XJson.Tests.Models;
using Xunit;

namespace Neemle.XJson.Tests;

public class JsonOptionsAndValidateTests
{
    [Fact]
    public void NamingPolicy_From_SourceGenerationOptions_Kebab_Lower_RoundTrip()
    {
        var sg = new JsonSourceGenerationOptionsAttribute
        {
            PropertyNamingPolicy = JsonKnownNamingPolicy.KebabCaseLower,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        var model = new WithWeirdNames { HomeCity = "Los Angeles" };
        string json = Json.Encode(model, sg);
        json.Should().Contain("\"home-city\"");

        var back = Json.Decode<WithWeirdNames>(json, sg);
        back.HomeCity.Should().Be("Los Angeles");
    }

    [Fact]
    public void Validate_Succeeds_And_Fails()
    {
        var a = new Address { Street = "X", City = "Y" };
        string ok = Json.Encode(a);
        Json.Validate<Address>(ok, out var value, out var error).Should().BeTrue();
        value.Should().NotBeNull();
        error.Should().BeNull();

        string bad = string.Empty; // invalid JSON (empty)
        Json.Validate<Address>(bad, out var value2, out var error2).Should().BeFalse();
        value2.Should().BeNull();
        error2.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void NullHandling_Omit_When_WhenWritingNull()
    {
        var m = new HasNullable { Optional = null, Count = null };
        string json = Json.Encode(m); // defaults to WhenWritingNull
        json.Should().NotContain("\"optional\"");
        json.Should().NotContain("\"count\"");
    }

    [Fact]
    public void NullHandling_WriteNull_When_Never()
    {
        var m = new HasNullable { Optional = null, Count = null };
        var opts = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            WriteIndented = false
        };
        string json = Json.Encode(m, opts);
        json.Should().Contain("\"Optional\":null");
        json.Should().Contain("\"Count\":null");
    }

    [Fact]
    public void Unsupported_PropertyType_Throws_On_Write_And_Read()
    {
        var u = new HasUnsupported { NotSupported = 1.23 };
        Action act = () => Json.Encode(u);
        act.Should().Throw<NotSupportedException>()
           .WithMessage("*Property type not supported: *");

        string json = "{\"notSupported\":1.23}"; // default camelCase property name
        Action act2 = () => Json.Decode<HasUnsupported>(json);
        act2.Should().Throw<NotSupportedException>()
            .WithMessage("*Property type not supported: *");
    }

    [Fact]
    public void NonAnnotated_Type_Throws()
    {
        var n = new NotAnnotated { Name = "Z" };
        Action w = () => Json.Encode(n);
        w.Should().Throw<NotSupportedException>()
            .WithMessage("*Type is not annotated with [XJson]*");

        Action r = () => Json.Decode<NotAnnotated>("{}");
        r.Should().Throw<NotSupportedException>()
            .WithMessage("*Type is not annotated with [XJson]*");
    }
}
