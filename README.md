JsonPath
========

Goal: extract values from JSON with single line expressions and simple CLR objects without using foreach/if contructs. Just like XPath for XML, but for JSON and type safe.

Nuget: https://www.nuget.org/packages/JsonPath

Requirements:

* .NET >= 3.5
* [Json.NET](https://www.nuget.org/packages/Newtonsoft.Json)

Extract the 42 from:

    [ "1st", "2nd", { "aString": "Hello World", "aNumber": 42 } ]

With C# expression:

    var json = new Node(data);
    int fourtytwo = json[2]["aNumber"];

If you want to be more explicit:

    int fourtytwo = json.AsList[2].AsDictionary["aNumber"].AsInt;
    var fourtytwo = (int)json.AsList[2].AsDictionary["aNumber"];
    var fourtytwo = json.AsList[2].AsDictionary["aNumber"].AsInt;

Invalid key: no exception, just a default 0, an empty string or an empty list:

    int zero = json[1000]["noNumber"];

Of course, you can foreach a dictionary (aka JS object):

    foreach (var pair in json[2]) {}

And iterate over a list (aka JS array):

    for (int i = 0; i < json.Count; i++) {
        string value = json[i];
    }

You can even LINQ it:

    json[2].Where(pair => pair.Key == "aNumber").First().Value
    (from x in json[2] where x.Key == "aNumber" select x.Value).First()

Dive deep into this JSON with a single line of code:

    var data = "[ { 
            aInt: 41, 
            bLong: 42000000000, 
            cBool: true, 
            dString: '43', 
            eFloat: 3.14159265358979323 
        }, { 
            fInt: 44, 
            gLong: 45000000000, 
            hString: "46"
        }, { 
            iList: [ 
                { jInt: 47, kString: '48' }, 
                { lInt: 49, mString: '50' }
            ], 
        }
    ]";

Using index notation:

     41  = (long) new JsonTree.Node(data)[0]["aInt"]
    "50" = (string) new JsonTree.Node(data)[2]["iList"][1]["mString"]

Using explicit notation:

     41  = (long) new JsonTree.Node(data).AsList[0].AsDictionary["aInt"].AsInt
    "50" = (string) new JsonTree.Node(data).AsList[2].AsDictionary["iList"].List[1].Dictionary["mString"].AsString

Using standard enumerators on CLR objects:

     41  = (long) new JsonTree.Node(data).AsList.ElementAt(0).AsDictionary.ElementAt(0).Value
    "50" = (string) new JsonTree.Node(data).AsList.ElementAt(2).AsDictionary.ElementAt(0).Value.AsList.ElementAt(1).AsDictionary.ElementAt(1).Value
