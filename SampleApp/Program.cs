using System.Text.Json;
using Neemle.XJson.Generated;
using Neemle.XJson.Sample;

var person = new Person
{
    Name = "Ada",
    Surname = "Lovelace",
    Age = 36,
    Address = new Address
    {
        Street = "12 St James's Square",
        City = "London"
    }
};

var opt = new JsonSerializerOptions()
{
    WriteIndented = false,
    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
};

string json = Json.Encode(person, opt);
Console.WriteLine("Serialized JSON:");
Console.WriteLine(json);

if (Json.Validate<Person>(json, out var decoded, out var error))
{
    Console.WriteLine();
    Console.WriteLine($"Round-trip: {decoded!.FullName} ({decoded.Age}) in {decoded.Address.City}");
}
else
{
    Console.WriteLine();
    Console.WriteLine($"Validation failed: {error}");
}
