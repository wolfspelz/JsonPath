using System;
using System.Linq;

namespace JsonPathDemo
{
    class Program
    {
        static void Main()
        {
            const string data = "[ \"1st\", \"2nd\", { \"aString\": \"Hello World\", \"aNumber\": 42 } ]";
            Console.WriteLine("data=" + data);

            var json = new JsonPath.Node(data);
            Console.WriteLine("var json = new JsonPath.Node(data)");

            Console.WriteLine("json[2][\"aNumber\"] = " + json[2]["aNumber"]);


            Console.WriteLine(Environment.NewLine + Environment.NewLine);
        }
    }
}
