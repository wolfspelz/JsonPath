using System;

namespace JsonPathDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("JsonPath Demo");
            Console.WriteLine("-------------");
            Console.WriteLine("");
            Console.WriteLine("Goal: extract values from JSON with single line expressions and simple CLR objects without using foreach/if contructs. Just like XPath fpr XML, but for JSON and type safe.");
            Console.WriteLine("");

            const string json = "[\"first\",\"second\",{\"aString\":\"Hello World\",\"aNumber\":42}]";
            Console.WriteLine("Given the JSON: " + json + Environment.NewLine);

            var root = new JsonPath.Node(json);

            Console.WriteLine("The 42 is the value of the aNumber-key in the map which is the second element of a list. JSON arrays/objects become C# lists/dictionaries." + Environment.NewLine);

            Console.WriteLine("Get to the value quickly:");
            int quick = root[2]["aNumber"];
            Console.WriteLine("  root[2][\"aNumber\"] = " + quick);

            Console.WriteLine("A bit more verbose:");
            int verbose = root.List[2].Dictionary["aNumber"];
            Console.WriteLine("  root.List[2].Dictionary[\"aNumber\"] = " + verbose);

            Console.WriteLine("No exceptions in index notation, just a default 0 or empty string:");
            int noidx = root[1000]["noNumber"];
            Console.WriteLine("  root[1000][\"noNumber\"] = " + noidx);

            Console.WriteLine("You can of course interate over a List:");
            for (int index = 0; index < root.AsList.Count; index++) {
                Console.WriteLine("  root[" + index + "] = " + root[index]);
            }

            Console.WriteLine("And a Dictionary:");
            foreach (var pair in root[2].AsDictionary) {
                Console.WriteLine("  root[2][\"" + pair.Key + "\"] = " + pair.Value);
            }

            Console.WriteLine("");
            Console.Write("Press <enter>");
            var rl = Console.ReadLine();
        }
    }
}
