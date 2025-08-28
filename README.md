# JsonPath

A .NET library for extracting values from JSON and XML with simple expressions, similar to XPath for XML but type-safe.

## Features

- **Simple JSON Navigation**: Extract values with intuitive syntax like `json[2]["aNumber"]`
- **Type-Safe Operations**: Built-in type conversions with `.AsInt`, `.AsString`, `.AsDictionary`, etc.
- **LINQ Integration**: Full support for LINQ queries on JSON data
- **XML Support**: Parse and navigate XML documents with the same syntax
- **No Exceptions**: Invalid paths return default values instead of throwing exceptions
- **Enumerable Support**: Iterate over JSON arrays and objects with foreach loops

## Quick Start

```csharp
using JsonPath;

// Parse JSON
var json = Node.FromJson("""
            [
                "1st",
                "2nd",
                {
                    "aString": "Hello World",
                    "aNumber": 42
                }
            ]
            """);

// Extract values
int number = json[2]["aNumber"];                    // 42
string text = json[2]["aString"];                   // "Hello World"
var explicitInt = json[2]["aNumber"].AsInt;         // 42

// Safe navigation - no exceptions for invalid paths
int invalid = json[1000]["noNumber"];               // 0 (default value)

// Iterate over collections
foreach (var pair in json[2]) {
    Console.WriteLine($"{pair.Key} = {pair.Value}");
}

// LINQ support
int result = json[2].Where(x => x.Key == "aNumber").Select(x => x.Value).First();
```

## XML Support

```csharp
var xml = Node.FromXml("""
            <root>
                <item>1st</item>
                <item>2nd</item>
                <item>
                    <aNumber>42</aNumber>
                </item>
            </root>
            """);
var number = xml[Xml.Children][2][Xml.Children]
    .AsList.FirstOrDefault(x => x[Xml.Name] == "aNumber")?[Xml.Text].AsInt;
```

## Installation

```
dotnet add package JsonPath
```

## Requirements

- .NET 8.0 or later
- Newtonsoft.Json

## License

MIT License
