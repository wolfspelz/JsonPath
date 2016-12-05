using System;
using System.Linq;

namespace PackageTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var json = "[ \"1st\", \"2nd\", { \"aString\": \"Hello World\", \"aNumber\": 42 } ]";
            Console.WriteLine("42 = " + (int)new JsonPath.Node(json).AsList[2].AsDictionary["aNumber"]);
            Console.WriteLine("42 = " + (int)new JsonPath.Node(json)[2]["aNumber"]);
            Console.WriteLine("42 = " + (int)new JsonPath.Node(json)[2].Where(x => x.Key == "aNumber").First().Value);

            Console.Write("Press <enter>"); Console.ReadLine();
        }
    }
}
