JsonPath
========

Goal: extract values from JSON with single line expressions and simple CLR objects without using foreach/if contructs. Just like XPath for XML, but for JSON and type safe.

Extract the 42 from:

    [ "first", "second", { "aString": "Hello World", "aNumber": 42 } ]

With C# expression:

    var json = new Node(data);
    int fourtytwo = json[2]["aNumber"];

If you want to be more verbose:

    int fourtytwo = json.List[2].Dictionary["aNumber"];
    var fourtytwo = (int)json.List[2].Dictionary["aNumber"];
    var fourtytwo = json.List[2].Dictionary["aNumber"].Int;

No exceptions in index notation, just a default 0 or empty string:

    int zero = root[1000]["noNumber"];

You can of course interate over a List:
    for (int i = 0; i < json.AsList.Count; i++) {
        string value = root[i];
    }

And a Dictionary:");
    foreach (var pair in json[2].AsDictionary) {
        //
    }

Dive deep into this JSON with a single line of code:

    var data = "[ { 
            aInt: 41, 
            bBool: true, 
            bLong: 42000000000, 
            cString: '43', 
            dFloat: 3.14159265358979323 
        }, { 
            aInt: 44, 
            bLong: 45000000000, 
            cString: "46"
        }, { 
            aList: [ 
                { aInt: 47, bString: '48' }, 
                { aInt: 49, bString: '50' } ], 
            bMap: { aInt: 51, bString: '52' } 
        }
    ]";

Using index notation:

     41  = (long) new JsonTree.Node(data)[0]["aInt"]
    "50" = (string) new JsonTree.Node(data)[2]["aList"][1]["bString"]

Using CLR notation (List and Dictionary):

     41  = (long) new JsonTree.Node(data).List[0].Dictionary["aInt"]
    "50" = (string) new JsonTree.Node(data).List[2].Dictionary["aList"].List[1].Dictionary["bString"]

Using standard enumerators on CLR objects:

     41  = (long) new JsonTree.Node(data).List.ElementAt(0).Dictionary.ElementAt(0).Value
    "50" = (string) new JsonTree.Node(data).List.ElementAt(2).Dictionary.ElementAt(0).Value.List.ElementAt(1).Dictionary.ElementAt(1).Value
