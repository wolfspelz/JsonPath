using System;
using System.Linq;

namespace JsonPath.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("JsonPath Demo");
            Console.WriteLine("-------------");
            Console.WriteLine("");
            Console.WriteLine("Goal: extract values from JSON with single line expressions and simple CLR objects without using foreach/if contructs. Just like XPath for XML, but for JSON and type safe.");
            Console.WriteLine("");

            string data = """
                [
                    "1st",
                    "2nd",
                    {
                        "aString": "Hello World",
                        "aNumber": 42
                    }
                ]
                """;
            Console.WriteLine("Given the JSON: " + Environment.NewLine + data + Environment.NewLine);

            var json = JsonPath.Node.FromJson(data);

            Console.WriteLine("The 42 is the value of the aNumber-key in the map which is the 3rd element of a list. JSON arrays/objects become C# lists/dictionaries." + Environment.NewLine);

            Console.WriteLine("Get to the value quickly:");
            int quick = json[2]["aNumber"];
            Console.WriteLine("  root[2][\"aNumber\"] = " + quick);
            Console.WriteLine("");

            Console.WriteLine("A bit more explicit:");
            var expl = json.AsList[2].AsDictionary["aNumber"].AsInt;
            Console.WriteLine("  root.AsList[2].AsDictionary[\"aNumber\"].AsInt = " + expl);
            Console.WriteLine("");

            Console.WriteLine("Invalid key: no exception, just a default 0, an empty string or an empty list:");
            int inval = json[1000]["noNumber"];
            Console.WriteLine("  root[1000][\"noNumber\"] = " + inval);
            Console.WriteLine("");

            Console.WriteLine("Of course, you can foreach a dictionary (aka JS object):");
            Console.WriteLine("  foreach (var pair in json[2]) {}");
            foreach (var pair in json[2]) {
                Console.WriteLine("      root[2][\"" + pair.Key + "\"] = " + pair.Value);
            }
            Console.WriteLine("");

            Console.WriteLine("And iterate over a list (aka JS array):");
            Console.WriteLine("  for (int i = 0; i < json.Count; i++) {}");
            for (int i = 0; i < json.Count; i++) {
                Console.WriteLine("      root[" + i + "] = " + json[i]);
            }
            Console.WriteLine("");

            Console.WriteLine("You can even LINQ it:");
            int linq = json[2].Where(x => x.Key == "aNumber").Select(x => x.Value).First();
            Console.WriteLine("  json[2].Where(x => x.Key == \"aNumber\").First().Value = " + linq);
            Console.WriteLine("or:");
            int linq2 = (from x in json[2] where x.Key == "aNumber" select x.Value).First();
            Console.WriteLine("  (from x in json[2] where x.Key == \"aNumber\" select x.Value).First() = " + linq2);
            Console.WriteLine("");

            Console.WriteLine("Now with XML:");
            Console.WriteLine("");

            const string xmlData = """
            <root>
                <item>1st</item>
                <item>2nd</item>
                <item>
                    <aString>Hello World</aString>
                    <aNumber>42</aNumber>
                </item>
            </root>
            """;
            Console.WriteLine("Given the XML: " + Environment.NewLine + xmlData + Environment.NewLine);

            var xml = JsonPath.Node.FromXml(xmlData);

            var quickXml = xml[Xml.Children][2][Xml.Children].AsList.FirstOrDefault(x => x[Xml.Name] == "aNumber")?[Xml.Text].AsInt;
            Console.WriteLine("  From the 3rd child, the item with name 'aNumber' and of that the inner text as int = " + quickXml);
            Console.WriteLine("");

            Console.WriteLine(""); Console.Write("Press <enter>"); Console.ReadLine();
        }
    }
}
