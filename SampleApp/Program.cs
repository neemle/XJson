using Neemle.XJson.Sample;
using Neemle.XJson.Generated;

var person = new Person
{
    Name = "Ada Lovelace",
    Age = 36,
    Address = new Address
    {
        Street = "12 St James's Square",
        City = "London"
    }
};

// Use the generated converters registered in Options to keep reflection out.
var json = XJsonGenerated.Serialize(person);
Console.WriteLine("Serialized JSON:");
Console.WriteLine(json);

var roundTrip = XJsonGenerated.Deserialize<Person>(json);
Console.WriteLine();
Console.WriteLine($"Round-trip: {roundTrip?.Name} ({roundTrip?.Age}) in {roundTrip?.Address.City}");
