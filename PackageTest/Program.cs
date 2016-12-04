using System;

namespace PackageTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("42 = " + new JsonPath.Node("[ \"first\", { \"aString\": \"Hello World\", \"aNumber\": 42 } ]").List[1].Dictionary["aNumber"].Int);

            Console.WriteLine("Press <ENTER>");
            var rl = Console.ReadLine();
        }
    }
}
