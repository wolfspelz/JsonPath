using System;
using System.Text.Json;
using JsonPath;

class Program
{
    static void Main()
    {
        // Sample JSON
        string json = @"{
            ""store"": {
                ""book"": [
                    { ""language"": ""German"", ""title"": ""Die Geschichte der Zukunft"", ""price"": 24.00 },
                    { ""language"": ""English"", ""title"": ""The History of the Future"", ""price"": 16.24 }
                ]
            }
        }";

        var node = JsonPath.Node.FromJson(json);
        Console.WriteLine("store.book.language(English).price: " + node.AsDictionary["store"]["book"].AsList.FirstOrDefault(n => n["language"] == "English")?["price"].AsFloat);
    }
}