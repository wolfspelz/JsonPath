using System;

namespace PackageTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("42 = " + (int)new JsonPath.Node("[ \"first\", \"second\", { \"aString\": \"Hello World\", \"aNumber\": 42 } ]").List[2].Dictionary["aNumber"]);
            Console.WriteLine("42 = " + (int)new JsonPath.Node("[ \"first\", \"second\", { \"aString\": \"Hello World\", \"aNumber\": 42 } ]")[2]["aNumber"]);

            Console.Write("Press <enter>");
            var rl = Console.ReadLine();
        }
    }
}
