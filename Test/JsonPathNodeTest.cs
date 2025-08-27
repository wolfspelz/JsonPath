using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace JsonPath.Test
{
    [TestClass]
    public class JsonPathNodeTest
    {
        [TestMethod]
        [TestCategory("JsonPath")]
        public void Deserializes_basic_types()
        {
            //var since = 43147.85388009260;
            //var date = System.DateTime.Now.AddDays(-since);
            //var days = (System.DateTime.Now - new System.DateTime(1899, 12, 30, 0, 0, 0, 0)).TotalDays;
            //var diff = days - since;
            //var secs = diff * 86400;

            Assert.IsTrue(Node.From("").IsString);
            Assert.IsTrue(Node.From("41").IsString);
            Assert.IsTrue(Node.FromJson("'41'").IsString);
            Assert.IsTrue(Node.From("true").IsString);
            Assert.IsTrue(Node.From("false").IsString);
            Assert.IsTrue(Node.From("41000000000").IsString);
            Assert.IsTrue(Node.From("3.14159265358979323").IsString);
            Assert.IsTrue(Node.FromJson("'https://www.galactic-developments.de/'").IsString);
            Assert.IsTrue(Node.From("\"" + new System.DateTime(2017, 1, 2, 3, 4, 5, 678).ToString("o") + "\"").IsString);
            Assert.IsTrue(Node.FromJson("{}").IsDictionary);
            Assert.IsTrue(Node.FromJson("{a:41}").IsDictionary);
            Assert.IsTrue(Node.FromJson("{a:null}").IsDictionary);
            Assert.IsTrue(Node.FromJson("{a:'41'}").IsDictionary);
            Assert.IsTrue(Node.FromJson("{a:41000000000}").IsDictionary);
            Assert.IsTrue(Node.FromJson("{a:3.1415927}").IsDictionary);
            Assert.IsTrue(Node.FromJson("{a:3.14159265358979323}").IsDictionary);
            Assert.IsTrue(Node.FromJson("{'a':41}").IsDictionary);
            Assert.IsTrue(Node.FromJson("{'a':'41'}").IsDictionary);
            Assert.IsTrue(Node.FromJson("{\"a\":\"41\"}").IsDictionary);
            Assert.IsTrue(Node.FromJson("[]").IsList);
            Assert.IsTrue(Node.FromJson("['a']").IsList);
            Assert.IsTrue(Node.FromJson("['a','b']").IsList);
            Assert.IsTrue(Node.FromJson("[41,42]").IsList);
            Assert.IsTrue(Node.FromJson("['a',42]").IsList);
            Assert.AreEqual(Node.FromJson("['a',42]").AsList.First().AsString, "a");
            Assert.AreEqual(Node.FromJson("['a',42]").AsList.ElementAt(1).AsInt, 42);
            Assert.IsTrue(Node.FromJson("{a:41}").AsDictionary.First().Value.IsInt);
            Assert.IsTrue(Node.FromJson("{a:'41'}").AsDictionary.First().Value.IsString);
            Assert.IsTrue(Node.FromJson("{a:41000000000}").AsDictionary.First().Value.IsInt);
            Assert.IsTrue(Node.FromJson("{a:3.1415927}").AsDictionary.First().Value.IsFloat);
            Assert.IsTrue(Node.FromJson("{a:3.14159265358979323}").AsDictionary.First().Value.IsFloat);
            //Assert.IsTrue(Node.FromJson("{a:.42}").AsDictionary.First().Value.IsFloat); // Broken since upgrade from Newtonsoft 9 to 10

            // Assert.AreEqual("x", Node.From("y").AsString);
            Assert.AreEqual(41L, Node.From("41").AsInt);
            Assert.AreEqual(true, Node.From("true").AsBool);
            Assert.AreEqual(false, Node.From("false").AsBool);
            Assert.AreEqual(41000000000L, Node.From("41000000000").AsInt);
            Assert.AreEqual(3.14159265358979323, Node.From("3.14159265358979323").AsFloat);
            Assert.AreEqual(41L, Node.FromJson("'41'").AsInt);
            Assert.AreEqual(new System.DateTime(2017, 1, 2, 3, 4, 5, 678), Node.FromJson("\"" + new System.DateTime(2017, 1, 2, 3, 4, 5, 678).ToString("o") + "\"").AsDate);
            Assert.AreEqual("true", Node.FromJson("{a:true}").AsDictionary["a"].AsString);
            Assert.AreEqual("false", Node.FromJson("{a:false}").AsDictionary["a"].AsString);
            Assert.AreEqual(41, Node.FromJson("{a:41}").AsDictionary.First().Value.AsInt);
            Assert.AreEqual(41, Node.FromJson("{a:41}").AsDictionary.First().Value.AsFloat);
            Assert.AreEqual("41", Node.FromJson("{a:'41'}").AsDictionary.First().Value.AsString);
            Assert.AreEqual(41000000000, Node.FromJson("{a:41000000000}").AsDictionary.First().Value.AsInt);
            Assert.AreEqual(3.1415927, Node.FromJson("{a:3.1415927}").AsDictionary.First().Value.AsFloat);
            Assert.AreEqual(3.1415927, Node.FromJson("{a:3.1415927}").AsDictionary["a"].AsFloat);
            Assert.AreEqual(3.14159265358979323, Node.FromJson("{a:3.14159265358979323}").AsDictionary.First().Value.AsFloat);
            Assert.AreEqual("true", Node.FromJson("{a:true}").AsDictionary["a"].AsString);
            Assert.AreEqual("", Node.FromJson("{a:null}").AsDictionary["a"].AsString);
            Assert.AreEqual(0, Node.FromJson("{a:null}").AsDictionary["a"].AsInt);
            Assert.AreEqual("41", Node.From("41").AsString);
            Assert.AreEqual("41000000000", Node.From("41000000000").AsString);
            Assert.AreEqual("3.14159265358979", Node.From("3.14159265358979").AsString);
            Assert.AreEqual(3.14159265358979, Node.From("3.14159265358979").AsFloat);
            Assert.AreEqual(1483326245, Node.FromJson("\"" + new System.DateTime(2017, 1, 2, 3, 4, 5).ToString("o") + "\"").AsInt);
        }

        [TestMethod]
        [TestCategory("JsonPath")]
        public void Converts_float_from_string_with_InvariantCulture()
        {
            Assert.AreEqual(3.14159265358979323, Node.FromJson("{\"a\":\"3.14159265358979323\"}").Dictionary.First().Value.AsFloat);
        }

        [TestMethod]
        [TestCategory("JsonPath")]
        public void Returns_empty_node_for_missing_dictionary_key()
        {
            Assert.AreEqual(0, Node.FromJson("{a:41}").AsDictionary.Get("b").AsInt);
        }

        [TestMethod]
        [TestCategory("JsonPath")]
        public void Returns_empty_node_for_missing_list_index()
        {
            Assert.AreEqual(0, Node.FromJson("[41,42]").AsList.Get(2).AsInt);
        }

        [TestMethod]
        [TestCategory("JsonPath")]
        public void Supports_alternative_notations_for_value_types()
        {
            // Arrange
            const string data = "{ a: '41', b: 42, c: true, d: 3.14, e: '2017-01-02T03:04:05.678' }";

            // Act
            var json = Node.FromJson(data);

            // Assert
            Assert.AreEqual("41", json.AsDictionary["a"].String);
            Assert.AreEqual(42, json.AsDictionary["b"].Int);
            Assert.AreEqual(true, json.AsDictionary["c"].Bool);
            Assert.AreEqual(3.14, json.AsDictionary["d"].Float);

            Assert.AreEqual("41", (string)json.AsDictionary["a"]);
            Assert.AreEqual(42, (long)json.AsDictionary["b"]);
            Assert.AreEqual(42, (int)json.AsDictionary["b"]);
            Assert.AreEqual(true, (bool)json.AsDictionary["c"]);
            Assert.AreEqual(3.14, (double)json.AsDictionary["d"]);

            string s = json.AsDictionary["a"];
            Assert.AreEqual("41", s);
            long l = json.AsDictionary["b"];
            Assert.AreEqual(42, l);
            int i = json.AsDictionary["b"];
            Assert.AreEqual(42, i);
            bool b = json.AsDictionary["c"];
            Assert.AreEqual(true, b);
            double d = json.AsDictionary["d"];
            Assert.AreEqual(3.14, d);
            System.DateTime e = json.AsDictionary["e"];
            Assert.AreEqual(new System.DateTime(2017, 1, 2, 3, 4, 5, 678), e);
        }

        [TestMethod]
        [TestCategory("JsonPath")]
        public void Supports_alternative_notations_for_structures()
        {
            // Arrange
            const string data = "{ a: 'b', c: [ 'c0', 'c1' ] }";
            var json = Node.FromJson(data);

            // Act, Assert
            Assert.AreEqual("c0", json.Dictionary["c"].List[0].AsString);
            Assert.AreEqual("c0", json.Dictionary["c"].List[0].AsString);
            Assert.AreEqual("c0", json["c"][0].AsString);
        }

        [TestMethod]
        [TestCategory("JsonPath")]
        public void Supports_List_enumeration()
        {
            // Arrange
            const string data = "{ a: 'b', c: [ 'c0', 'c1' ] }";

            // Act
            var json = Node.FromJson(data);

            // Assert
            for (int i = 0; i < json["c"].Count; i++) {
                Assert.AreEqual("c" + i, (string)json["c"][i]);
            }
        }

        [TestMethod]
        [TestCategory("JsonPath")]
        public void Supports_Dictionary_enumeration()
        {
            // Arrange
            const string data = "{ a: 'b', c: [ 'c0', 'c1' ] }";

            // Act
            var json = Node.FromJson(data);

            // Assert
            foreach (var pair in json) {
                if (pair.Key == "a") {
                    Assert.AreEqual("b", (string)pair.Value);
                }
                if (pair.Key == "c") {
                    Assert.IsTrue(pair.Value.IsList);
                }
            }
        }

        [TestMethod]
        [TestCategory("JsonPath")]
        public void Node_From_string_string_Dictionary()
        {
            // Arrange
            var data = new Dictionary<string, string> { ["a"] = "b", ["c"] = "d", };

            // Act
            var node = Node.From(data);

            // Assert
            Assert.AreEqual("b", (string)node["a"]);
            Assert.AreEqual("d", (string)node["c"]);
        }

        [TestMethod]
        [TestCategory("JsonPath")]
        public void Node_From_string_long_Dictionary()
        {
            // Arrange
            var data = new Dictionary<string, long> { ["a"] = 42, ["c"] = 42000000000, };

            // Act
            var node = Node.From(data);

            // Assert
            Assert.AreEqual(42, (long)node["a"]);
            Assert.AreEqual(42000000000, (long)node["c"]);
        }

        [TestMethod]
        [TestCategory("JsonPath")]
        public void Supports_Linq()
        {
            // Arrange
            const string data = "{ a: 'b', c: [ 'c0', 'c1' ] }";

            // Act
            var json = Node.FromJson(data);

            // Assert
            Assert.AreEqual("b", (string)json.Where(x => x.Key == "a").Select(x => x.Value).First());
            Assert.AreEqual("b", (string)(from x in json where x.Key == "a" select x.Value).First());
        }

        [TestMethod]
        [TestCategory("JsonPath")]
        public void Deserializes_a_typical_json_structure()
        {
            var data = @"
[
  {
    aInt: 41,
    bBool: true,
    bLong: 42000000000,
    cString: '43',
    eDate: '2017-01-02T03:04:05.678',
    dFloat: 3.14159265358979323
  },
  {
    aInt: 44,
    bLong: 45000000000,
    cString: ""46""
  },
  {
    aList:
    [
      {
        aInt: 47,
        bString: '48'
      },
      {
        aInt: 49,
        bString: '50'
      }
    ],
    bMap:
    {
      aInt: 51,
      bString: '52'
    }
  }
]
";
            // Act
            var json = Node.FromJson(data);

            // Assert
            Node? lastListItem = null;
            string? lastMapItemKey = null;
            Node? lastMapItemValue = null;
            foreach (var item in json.List) {
                lastListItem = item;
            }
            foreach (var pair in json.List.First().Dictionary) {
                lastMapItemKey = pair.Key;
                lastMapItemValue = pair.Value;
            }

            // ReSharper disable once PossibleNullReferenceException
            Assert.AreEqual(lastListItem?.Dictionary.Count, 2);
            Assert.AreEqual(lastMapItemKey, "dFloat");
            // ReSharper disable once PossibleNullReferenceException
            Assert.AreEqual(lastMapItemValue?.AsFloat, 3.14159265358979323);

            // Act/Assert
            Assert.AreEqual(json.List.ElementAt(0).Dictionary.ElementAt(0).Key, "aInt");
            Assert.AreEqual(json.List.ElementAt(0).Dictionary.ElementAt(0).Value.AsInt, 41);
            Assert.AreEqual(json.List.ElementAt(0).Dictionary.ElementAt(1).Key, "bBool");
            Assert.AreEqual(json.List.ElementAt(0).Dictionary.ElementAt(1).Value.AsBool, true);
            Assert.AreEqual(json.List.ElementAt(0).Dictionary.ElementAt(2).Key, "bLong");
            Assert.AreEqual(json.List.ElementAt(0).Dictionary.ElementAt(2).Value.AsInt, 42000000000);
            Assert.AreEqual(json.List.ElementAt(0).Dictionary.ElementAt(3).Key, "cString");
            Assert.AreEqual(json.List.ElementAt(0).Dictionary.ElementAt(3).Value.AsString, "43");
            Assert.AreEqual(json.List.ElementAt(0).Dictionary.ElementAt(4).Key, "eDate");
            Assert.AreEqual(json.List.ElementAt(0).Dictionary.ElementAt(4).Value.AsDate, new System.DateTime(2017, 1, 2, 3, 4, 5, 678));
            Assert.AreEqual(json.List.ElementAt(0).Dictionary.ElementAt(5).Key, "dFloat");
            Assert.AreEqual(json.List.ElementAt(0).Dictionary.ElementAt(5).Value.AsFloat, 3.14, 0.01);

            Assert.AreEqual(json.List.ElementAt(0).Dictionary.ElementAt(5).Value.AsFloat, 3.14159265358979323);
            Assert.AreEqual(json.List[0].Dictionary.ElementAt(5).Value.AsFloat, 3.14159265358979323);

            Assert.AreEqual(json.List.ElementAt(1).Dictionary.ElementAt(0).Key, "aInt");
            Assert.AreEqual(json.List.ElementAt(1).Dictionary.ElementAt(0).Value.AsInt, 44);
            Assert.AreEqual(json.List.ElementAt(1).Dictionary.ElementAt(1).Key, "bLong");
            Assert.AreEqual(json.List.ElementAt(1).Dictionary.ElementAt(1).Value.AsInt, 45000000000);
            Assert.AreEqual(json.List.ElementAt(1).Dictionary.ElementAt(2).Key, "cString");
            Assert.AreEqual(json.List.ElementAt(1).Dictionary.ElementAt(2).Value.AsString, "46");

            Assert.AreEqual(json.List.ElementAt(2).Dictionary.ElementAt(0).Key, "aList");
            Assert.AreEqual(json.List.ElementAt(2).Dictionary.ElementAt(0).Value.List.Count, 2);
            Assert.AreEqual(json.List.ElementAt(2).Dictionary.ElementAt(0).Value.List.ElementAt(0).Dictionary.ElementAt(0).Key, "aInt");
            Assert.AreEqual(json.List.ElementAt(2).Dictionary.ElementAt(0).Value.List.ElementAt(0).Dictionary.ElementAt(0).Value.AsInt, 47);
            Assert.AreEqual(json.List.ElementAt(2).Dictionary.ElementAt(0).Value.List.ElementAt(0).Dictionary.ElementAt(1).Key, "bString");
            Assert.AreEqual(json.List.ElementAt(2).Dictionary.ElementAt(0).Value.List.ElementAt(0).Dictionary.ElementAt(1).Value.AsString, "48");
            Assert.AreEqual(json.List.ElementAt(2).Dictionary.ElementAt(0).Value.List.ElementAt(1).Dictionary.ElementAt(0).Key, "aInt");
            Assert.AreEqual(json.List.ElementAt(2).Dictionary.ElementAt(0).Value.List.ElementAt(1).Dictionary.ElementAt(0).Value.AsInt, 49);
            Assert.AreEqual(json.List.ElementAt(2).Dictionary.ElementAt(0).Value.List.ElementAt(1).Dictionary.ElementAt(1).Key, "bString");

            Assert.AreEqual(json.List.ElementAt(2).Dictionary.ElementAt(0).Value.List.ElementAt(1).Dictionary.ElementAt(1).Value.AsString, "50");
            Assert.AreEqual(json.List[2].Dictionary["aList"].List[1].Dictionary["bString"].AsString, "50");

            Assert.AreEqual(json.List.ElementAt(2).Dictionary.ElementAt(1).Key, "bMap");
            Assert.AreEqual(json.List.ElementAt(2).Dictionary.ElementAt(1).Value.Dictionary.Count, 2);
        }

        [TestMethod]
        [TestCategory("JsonPath")]
        public void Patch()
        {
            var data = @"
{
    'StringStay': 'StringStayValue',
    'StringSet': 'StringSetValue',
    'StringImplicitSet': 'StringImplicitSetValue',
    'StringDelete': 'StringDeleteValue',
    'Int1': 41,
    'Bool1': true,
    'DictStay': {
        'DictStayString1': 'DictStayString1Value',
        'DictStayInt1': 41,
        'DictStayBool1': true
    },
    'DictSet': {
        'DictSetString1': 'DictSetString1Value',
        'DictSetInt1': 41,
        'DictSetBool1': true
    },
    'DictChange': {
        'DictChangeString1': 'DictChangeString1Value',
        'DictChangeInt1': 41,
        'DictChangeBool1': true
    },
    'DictExplicitDescend': {
        'DictExplicitDescendString1': 'DictExplicitDescendString1Value',
        'DictExplicitDescendInt1': 41,
    },
    'DictDelete': {
        'DictDeleteString1': 'DictDeleteString1Value',
        'DictDeleteInt1': 41,
        'DictDeleteBool1': true,
    },
    'DictRemove': {
        'DictRemoveString1': 'DictRemoveString1Value',
        'DictRemoveString2': 'DictRemoveString2Value',
        'DictRemoveString3': 'DictRemoveString3Value'
    },
    'ListStay': [
        'ListStayValue1',
        'ListStayValue2',
        'ListStayValue3'
    ],
    'ListSet': [
        'ListSetValue1',
        'ListSetValue2',
        'ListSetValue3'
    ],
    'ListDelete': [
        'ListDeleteValue1',
        'ListDeleteValue2',
        'ListDeleteValue3'
    ],
    'ListAdd': [
        'ListAddValue1',
        'ListAddValue2'
    ],
    'ListRemove': [
        'ListRemoveValue1',
        'ListRemoveValue2'
    ],
    'Nested': {
        'NestedDictChange': {
            'NestedDictChangeString1': 'NestedDictChangeString1ValueChanged',
            'NestedDictChangeInt1': 42,
            'NestedDictChangeBool1': false
        },
    },
    'DeelplyNested': {
        'DeelplyNested1': {
            'DeelplyNested2': {
                'DeelplyNestedDictChange': {
                    'DeelplyNestedDictChangeString1': 'DeelplyNestedDictChangeString1Value',
                }
            }
        }
    }
}
";

            var patchData = @"
{
    'StringImplicitSet': 'StringImplicitSetValueChanged',
    'DictCreatedOnDemand': {
        'DictCreatedOnDemandString1': 'DictCreatedOnDemandString1Value',
    },
    'StringSet|set': 'StringSetValueChanged',
    'StringAdd': 'StringAddValue',
    'StringDelete|delete': true,
    'DictChange': {
        'DictChangeString1': 'DictChangeString1ValueChanged',
        'DictChangeInt1': 42,
        'DictChangeInt2': 43,
        'DictChangeBool1': false
    },
    'DictExplicitDescend|descend': {
        'DictExplicitDescendString1': 'DictExplicitDescendString1ValueChanged',
        'DictExplicitDescendInt1': 42,
        'DictExplicitDescendInt2': 43,
    },
    'DictSet|replace': {
        'DictSetString1': 'DictSetString1Value',
    },
    'DictDelete|delete': true,
    'DictRemove|remove': [ 'DictRemoveString2' ],
    'ListSet|replace': [ 42, 43 ],
    'ListDelete|delete': true,
    'ListAdd|add': [ 'ListAddValue3', 'ListAddValue4' ],
    'ListRemove|remove': [ 'ListRemoveValue1' ],
    'Nested': {
        'NestedDictChange': {
            'NestedDictChangeString1': 'NestedDictChangeString1ValueChanged',
            'NestedDictChangeInt1': 42,
            'NestedDictChangeBool1': false
        },
    },
    'DeelplyNested': {
        'DeelplyNested1': {
            'DeelplyNested2': {
                'DeelplyNestedDictChange': {
                    'DeelplyNestedDictChangeString1': 'DeelplyNestedDictChangeString1ValueChanged',
                },
            },
        },
    },
}";

            var json = Node.FromJson(data);
            var patch = Node.FromJson(patchData);

            // Act
            json.Patch(patch);

            // Assert
            Assert.AreEqual(41, (int)json["Int1"]);
            Assert.AreEqual(true, (bool)json["Bool1"]);

            Assert.AreEqual("StringStayValue", (string)json["StringStay"]);
            Assert.AreEqual("StringSetValueChanged", (string)json["StringSet"]);
            Assert.AreEqual("StringImplicitSetValueChanged", (string)json["StringImplicitSet"]);
            Assert.AreEqual("StringAddValue", (string)json["StringAdd"]);
            Assert.AreEqual("", (string)json["StringDelete"]);

            Assert.AreEqual(3, json["DictStay"].AsDictionary.Count);
            Assert.AreEqual("DictStayString1Value", (string)json["DictStay"]["DictStayString1"]);
            Assert.AreEqual(41, (int)json["DictStay"]["DictStayInt1"]);
            Assert.AreEqual(true, (bool)json["DictStay"]["DictStayBool1"]);

            Assert.AreEqual(1, json["DictSet"].AsDictionary.Count);
            Assert.AreEqual("DictSetString1Value", (string)json["DictSet"]["DictSetString1"]);

            Assert.AreEqual(1, json["DictCreatedOnDemand"].AsDictionary.Count);
            Assert.AreEqual("DictCreatedOnDemandString1Value", (string)json["DictCreatedOnDemand"]["DictCreatedOnDemandString1"]);

            Assert.AreEqual(4, json["DictChange"].AsDictionary.Count);
            Assert.AreEqual("DictChangeString1ValueChanged", (string)json["DictChange"]["DictChangeString1"]);
            Assert.AreEqual(42, (int)json["DictChange"]["DictChangeInt1"]);
            Assert.AreEqual(43, (int)json["DictChange"]["DictChangeInt2"]);
            Assert.AreEqual(false, (bool)json["DictChange"]["DictChangeBool1"]);

            Assert.AreEqual(3, (int)json["DictExplicitDescend"].AsDictionary.Count);
            Assert.AreEqual("DictExplicitDescendString1ValueChanged", (string)json["DictExplicitDescend"]["DictExplicitDescendString1"]);
            Assert.AreEqual(42, (int)json["DictExplicitDescend"]["DictExplicitDescendInt1"]);
            Assert.AreEqual(43, (int)json["DictExplicitDescend"]["DictExplicitDescendInt2"]);

            Assert.AreEqual(0, json["DictDelete"].AsDictionary.Count);

            Assert.AreEqual(2, json["DictRemove"].AsDictionary.Count);
            Assert.AreEqual("DictRemoveString1Value", (string)json["DictRemove"]["DictRemoveString1"]);
            Assert.AreEqual("DictRemoveString3Value", (string)json["DictRemove"]["DictRemoveString3"]);

            Assert.AreEqual(3, json["ListStay"].AsList.Count);

            Assert.AreEqual(2, json["ListSet"].AsList.Count);
            Assert.AreEqual(42, (int)json["ListSet"][0]);
            Assert.AreEqual(43, (int)json["ListSet"][1]);

            Assert.AreEqual(0, json["ListDelete"].AsList.Count);

            Assert.AreEqual(4, json["ListAdd"].AsList.Count);

            Assert.AreEqual(1, json["ListRemove"].AsList.Count);
            Assert.AreEqual("ListRemoveValue2", (string)json["ListRemove"][0]);

            Assert.AreEqual(3, json["Nested"]["NestedDictChange"].AsDictionary.Count);
            Assert.AreEqual("NestedDictChangeString1ValueChanged", (string)json["Nested"]["NestedDictChange"]["NestedDictChangeString1"]);
            Assert.AreEqual(42, (int)json["Nested"]["NestedDictChange"]["NestedDictChangeInt1"]);
            Assert.AreEqual(false, (bool)json["Nested"]["NestedDictChange"]["NestedDictChangeBool1"]);

            Assert.AreEqual(1, json["DeelplyNested"]["DeelplyNested1"]["DeelplyNested2"]["DeelplyNestedDictChange"].AsDictionary.Count);
            Assert.AreEqual("DeelplyNestedDictChangeString1ValueChanged", (string)json["DeelplyNested"]["DeelplyNested1"]["DeelplyNested2"]["DeelplyNestedDictChange"]["DeelplyNestedDictChangeString1"]);

        }

        [TestMethod]
        [TestCategory("JsonPath")]
        public void Dictionary_ctor()
        {
            // Arrange

            // Act
            var json = Node.From(new Dictionary<string, object?> { ["string"] = "a", ["int"] = 42, ["long"] = 42000000000, ["double"] = 3.14, ["bool"] = true, ["date"] = new DateTime(2019, 2, 12, 1, 2, 3), });

            // Assert
            Assert.AreEqual("a", json["string"].String);
            Assert.AreEqual(42, json["int"].Int);
            Assert.AreEqual(42000000000, json["long"].Int);
            Assert.AreEqual(3.14, json["double"].Float);
            Assert.AreEqual(true, json["bool"].Bool);
            Assert.AreEqual(new DateTime(2019, 2, 12, 1, 2, 3), json["date"].Date);
        }

        [TestMethod]
        [TestCategory("JsonPath")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0028:Simplify collection initialization", Justification = "<Pending>")]
        public void Dictionary_operatorNode()
        {
            var dict = new Dictionary();
            dict["a"] = Node.From(41);
            dict["b"] = Node.From("42");
            var node = (Node)dict;
            Assert.AreEqual(41L, Node.FromJson(node.ToJson())["a"].Int);
            Assert.AreEqual("42", Node.FromJson(node.ToJson())["b"].String);
        }

        [TestMethod]
        [TestCategory("JsonPath")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0028:Simplify collection initialization", Justification = "<Pending>")]
        public void List_operatorNode()
        {
            var list = new List();
            list.Add(Node.From(41));
            list.Add(Node.From("42"));
            var node = (Node)list;
            Assert.AreEqual(41L, Node.FromJson(node.ToJson())[0].Int);
            Assert.AreEqual("42", Node.FromJson(node.ToJson())[1].String);
        }

        [TestMethod]
        [TestCategory("JsonPath")]
        public void Nulls()
        {
            // Null scalar:
            Assert.AreEqual(true, new Node(Node.Type.Empty).IsNull);
            Assert.AreEqual("", new Node(Node.Type.Empty).AsString);
            Assert.AreEqual("", new Node(Node.Type.Empty).ToJson());

            Assert.AreEqual(true, Node.FromJson("").IsNull);
            Assert.AreEqual("", Node.FromJson("").AsString);
            Assert.AreEqual("", Node.FromJson("").ToJson());

            Assert.AreEqual(true, Node.FromJson("null").IsNull);
            Assert.AreEqual("", Node.FromJson("null").AsString);
            Assert.AreEqual("", Node.FromJson("null").ToJson());

            {// List containing null:
                var node = Node.FromJson("[null, 1, null]");
                Assert.AreEqual(true, node.List[0].IsNull);
                Assert.AreEqual("", node.List[0].AsString);
                Assert.AreEqual("", node.List[0].ToJson());
                Assert.AreEqual(false, node.List[1].IsNull);
                Assert.AreEqual("1", node.List[1].AsString);
                Assert.AreEqual("1", node.List[1].ToJson());
                Assert.AreEqual(true, node.List[2].IsNull);
                Assert.AreEqual("", node.List[2].AsString);
                Assert.AreEqual("", node.List[2].ToJson());
            }
            {// List containing null - JSON roundtrip:
                var node = Node.FromJson(Node.FromJson("[null, 1, null]").ToJson());
                Assert.AreEqual(true, node.List[0].IsNull);
                Assert.AreEqual("", node.List[0].AsString);
                Assert.AreEqual("", node.List[0].ToJson());
                Assert.AreEqual(false, node.List[1].IsNull);
                Assert.AreEqual("1", node.List[1].AsString);
                Assert.AreEqual("1", node.List[1].ToJson());
                Assert.AreEqual(true, node.List[2].IsNull);
                Assert.AreEqual("", node.List[2].AsString);
                Assert.AreEqual("", node.List[2].ToJson());
            }

            {// Dictionary containing null:
                var node = Node.FromJson("{\"a\": null, \"b\": 1, \"c\": null}");
                Assert.AreEqual(true, node.Dictionary["a"].IsNull);
                Assert.AreEqual("", node.Dictionary["a"].AsString);
                Assert.AreEqual("", node.Dictionary["a"].ToJson());
                Assert.AreEqual(false, node.Dictionary["b"].IsNull);
                Assert.AreEqual("1", node.Dictionary["b"].AsString);
                Assert.AreEqual("1", node.Dictionary["b"].ToJson());
                Assert.AreEqual(true, node.Dictionary["c"].IsNull);
                Assert.AreEqual("", node.Dictionary["c"].AsString);
                Assert.AreEqual("", node.Dictionary["c"].ToJson());
            }
            { // Dictionary containing null - JSON roundtrip:
                var node = Node.FromJson(Node.FromJson("{\"a\": null, \"b\": 1, \"c\": null}").ToJson());
                Assert.AreEqual(true, node.Dictionary["a"].IsNull);
                Assert.AreEqual("", node.Dictionary["a"].AsString);
                Assert.AreEqual("", node.Dictionary["a"].ToJson());
                Assert.AreEqual(false, node.Dictionary["b"].IsNull);
                Assert.AreEqual("1", node.Dictionary["b"].AsString);
                Assert.AreEqual("1", node.Dictionary["b"].ToJson());
                Assert.AreEqual(true, node.Dictionary["c"].IsNull);
                Assert.AreEqual("", node.Dictionary["c"].AsString);
                Assert.AreEqual("", node.Dictionary["c"].ToJson());
            }
        }

    }
}
