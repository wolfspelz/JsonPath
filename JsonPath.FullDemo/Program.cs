using System;
using System.Collections.Generic;
using System.Linq;

namespace JsonPathDemo
{
    class Program
    {
        static async System.Threading.Tasks.Task Main()
        {
            Console.WriteLine("JsonPath Demo");
            Console.WriteLine("-------------");
            Console.WriteLine("");
            Console.WriteLine("Goal: extract values from JSON with single line expressions and simple CLR objects without using foreach/if contructs. Just like XPath for XML, but for JSON and type safe.");
            Console.WriteLine("");

            const string data = "[ \"1st\", \"2nd\", { \"aString\": \"Hello World\", \"aNumber\": 42 } ]";
            Console.WriteLine("Given the JSON: " + data + Environment.NewLine);

            var json = new JsonPath.Node(data);

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
            int linq2 = (from x in json[2] where x.Key == "aNumber" select x.Value).First();
            Console.WriteLine("  (from x in json[2] where x.Key == \"aNumber\" select x.Value).First() = " + linq2);

            Console.WriteLine(Environment.NewLine + Environment.NewLine + "Now, the TextProvider:" + Environment.NewLine);

            var Text = new JsonPath.TextProvider(new JsonPath.MemoryDataProvider(), "JsonPathDemo", "en-US", "IndexPage");
            // For demo purposes using an in-memory data provider
            await Text.Set("Key1", "{ 'myHeader': { 'myTitle': 'Page Title', 'myList': ['Text1', 'Text2'] } }");
            Console.WriteLine("Use this in a Controller or the code page of the Razor view to create a PageModel property (rather than the 'var' as in this example). Then in the view do:" + Environment.NewLine);
            Console.WriteLine("<h1>text.String(key: \"Key1\", path: \"myHeader/myTitle\")<h1>");
            Console.WriteLine("<ul>");
            Console.WriteLine("  @foreach (var listElem in text.String(key: \"Key1\", path: \"myHeader/myTitle\") {");
            Console.WriteLine("    <li>@listElem</li>");
            Console.WriteLine("  }");
            Console.WriteLine("</ul>");
            Console.WriteLine(Environment.NewLine + "which will of course show:" + Environment.NewLine);
            Console.WriteLine("<h1>" + await Text.String(key: "Key1", path: "myHeader/myTitle") + "<h1>");
            Console.WriteLine("<ul>");
            foreach (var listElem in await Text.List(key: "Key1", path: "myHeader/myList")) {
                Console.WriteLine("    <li>" + listElem + "</li>");
            }
            Console.WriteLine("</ul>" + Environment.NewLine);

            Console.WriteLine("In addition to Text.String() and Text.List() there is Text.Dictionary() for (well) JSON objects and Text.Json() for raw data" + Environment.NewLine);


            Console.WriteLine(""); Console.WriteLine("");
        }
    }
}
