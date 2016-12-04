JsonPath
========

Inspect values in JSON data with single line expressions without using foreach/if to extract values from JSON. 

Extract the 42 from:

    '[ "first ", { "aString": "HelloWorld", "aNumber": 42 } ]'

With C# expressions:

    long fourtytwo = json.List[1].Dictionary["aNumber"];
    var fourtytwo = (long)json.List[1].Dictionary["aNumber"];
    var fourtytwo = json.List[1].Dictionary["aNumber"].Int;

Or you may use Javascript notation (Array/Object instead of List/Dictionary ) for the same result: 

    var fourtytwo = (long)json.Array[1].Object["aNumber"];

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
            cString: \"46\"
        }, { 
            aList: [ 
                { aInt: 47, bString: '48' }, 
                { aInt: 49, bString: '50' } ], 
            bMap: { aInt: 51, bString: '52' } 
        }
    ]";

Using JavaScript notation (keywords Array and Object):

     41  = (long) new JsonTree.Node(data).Array[0].Object["aInt"]
    "50" = (string) new JsonTree.Node(data).Array[2].Object["aList"].Array[1].Object["bString"]

Using CLR notation (List and Dictionary):

     41  = (long) new JsonTree.Node(data).List[0].Dictionary["aInt"]
    "50" = (string) new JsonTree.Node(data).List[2].Dictionary["aList"].List[1].Dictionary["bString"]

Using standard enumerators on CLR objects:

     41  = (long) new JsonTree.Node(data).List.ElementAt(0).Dictionary.ElementAt(0).Value
    "50" = (string) new JsonTree.Node(data).List.ElementAt(2).Dictionary.ElementAt(0).Value.List.ElementAt(1).Dictionary.ElementAt(1).Value

Using fail save accessors which do not throw exceptions for missing indexes:

     41  = (long) new JsonTree.Node(data).List.Get(0).Dictionary.Get("aInt")
    "50" = (string) new JsonTree.Node(data).List.Get(2).Dictionary.Get("aList").List.Get(1).Dictionary.Get("bString")
