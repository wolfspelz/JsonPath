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
            Console.WriteLine("Goal: inspect values in JSON with single line expressions and simple CLR objects without using foreach/if to extract values from JSON");
            Console.WriteLine("");

            const string json = "[ \"first\", { \"aString\": \"Hello World\", \"aNumber\": 42 } ]";
            Console.WriteLine("Given the JSON: " + json);
            Console.WriteLine("");

            var root = new JsonPath.Node(json);

            Console.WriteLine("The 42 is the value of the aNumber-key in the map which is the second element of a list. JSON arrays/objects become C# lists/dictionaries.");
            Console.WriteLine("");

            Console.WriteLine("Get to the value quickly:");
            var aNumber = root.List[1].Dictionary["aNumber"].Int;
            Console.WriteLine("  root.List[1].Dictionary[\"aNumber\"].Int = " + aNumber);
            Console.WriteLine("");

            Console.WriteLine("The same in JavaScript notation:");
            var aNumber2 = root.Array[1].Object["aNumber"].Int;
            Console.WriteLine("  root.Array[1].Object[\"aNumber\"].Int = " + aNumber2);
            Console.WriteLine("");

            Console.WriteLine("The same with fail safe getters:");
            var aNumber3 = root.Array.Get(1).Object.Get("aNumber").Int;
            Console.WriteLine("  root.Array.Get(1).Object.Get(\"aNumber\").Int = " + aNumber3);
            Console.WriteLine("");

            Console.WriteLine("Fail safe getters do not throw exceptions for invalid indexes:");
            var aNumber4 = root.Array.Get(2).Object.Get("noNumber").Int;
            Console.WriteLine("  root.Array.Get(2).Object.Get(\"noNumber\").Int = " + aNumber4);
            Console.WriteLine("");

            Console.WriteLine("Press <ENTER>");
            var rl = Console.ReadLine();
        }
    }
}
