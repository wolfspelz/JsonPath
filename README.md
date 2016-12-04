JsonPath
========

Inspect values in JSON data with single line expressions without using foreach/if to extract values from JSON. 

Extract the 42 from:

    '[ "first ", { "aString": "HelloWorld", "aNumber": 42 } ]'

With C# expression:

    var fourtytwo = json.List[1].Dictionary["aNumber"].Int;

Or you may use Javascript notation (Array/Object instead of List/Dictionary ) for the same result: 

    var fourtytwo = json.Array[1].Object["aNumber"].Int;

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

     41  = new JsonTree.Node(data).Array[0].Object["aInt"].Int
    "50" = new JsonTree.Node(data).Array[2].Object["aList"].Array[1].Object["bString"].String

Using CLR notation (List and Dictionary):

     41  = new JsonTree.Node(data).List[0].Dictionary["aInt"].Int
    "50" = new JsonTree.Node(data).List[2].Dictionary["aList"].List[1].Dictionary["bString"].String

Using standard enumerators on CLR objects:

     41  = new JsonTree.Node(data).List.ElementAt(0).Dictionary.ElementAt(0).Value.Int
    "50" = new JsonTree.Node(data).List.ElementAt(2).Dictionary.ElementAt(0).Value.List.ElementAt(1).Dictionary.ElementAt(1).Value.String

Using fail save accessors which do not throw exceptions for missing indexes:

     41  = new JsonTree.Node(data).List.Get(0).Dictionary.Get("aInt").Int
    "50" = new JsonTree.Node(data).List.Get(2).Dictionary.Get("aList").List.Get(1).Dictionary.Get("bString").String
