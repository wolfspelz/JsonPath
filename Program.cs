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
            int aNumber1 = root.List[1].Dictionary["aNumber"];
            Console.WriteLine("  root.List[1].Dictionary[\"aNumber\"] = " + aNumber1);
            Console.WriteLine("");

            Console.WriteLine("The same in JavaScript notation:");
            int aNumber2 = root.Array[1].Object["aNumber"];
            Console.WriteLine("  root.Array[1].Object[\"aNumber\"] = " + aNumber2);
            Console.WriteLine("");

            Console.WriteLine("The same with fail safe getters:");
            int aNumber3 = root.Array.Get(1).Object.Get("aNumber");
            Console.WriteLine("  root.Array.Get(1).Object.Get(\"aNumber\") = " + aNumber3);
            Console.WriteLine("");

            Console.WriteLine("Fail safe getters do not throw exceptions for invalid indexes:");
            int aNumber4 = root.Array.Get(2).Object.Get("noNumber");
            Console.WriteLine("  root.Array.Get(2).Object.Get(\"noNumber\") = " + aNumber4);
            Console.WriteLine("");

            Console.WriteLine("Press <ENTER>");
            var rl = Console.ReadLine();
        }
    }
}
