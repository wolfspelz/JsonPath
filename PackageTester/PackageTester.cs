using System;
using System.Text.Json;
using JsonPath;

class Program
{
    static void Main()
    {
        // Sample JSON
        string json = """
        {
            "store": {
            "books": [
                { "language": "German", "title": "Die Geschichte der Zukunft", "price": 24.00 },
                { "language": "English", "title": "The History of the Future", "price": 16.24 }
            ]
            }
        }
        """;

        var jsonNode = JsonPath.Node.FromJson(json);
        Console.WriteLine("JSON: store.books.language(English).price: " + jsonNode.AsDictionary["store"]["books"].AsList.FirstOrDefault(n => n["language"] == "English")?["price"].AsFloat);

        // Sample YAML
        string yaml = """
        store:
          books:
            - language: German
              title: Die Geschichte der Zukunft
              price: 24.00
            - language: English
              title: The History of the Future
              price: 16.24
        """;

        var yamlNode = JsonPath.Node.FromYaml(yaml);
        Console.WriteLine("YAML: store.books.language(English).price: " + yamlNode.AsDictionary["store"]["books"].AsList.FirstOrDefault(n => n["language"] == "English")?["price"].AsFloat);
    }
}