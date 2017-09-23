using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JsonPath;
using System.Linq;

namespace Test.JsonPath
{
    [TestClass]
    public class Test
    {
        [TestClass]
        public class JsonPathDeserializerTest
        {
            [TestMethod]
            public void DeserializeBasicTypes()
            {
                Assert.IsTrue(new Node("").IsEmpty);
                Assert.IsTrue(new Node("41").IsInt);
                Assert.IsTrue(new Node("true").IsBool);
                Assert.IsTrue(new Node("false").IsBool);
                Assert.IsTrue(new Node("'41'").IsString);
                Assert.IsTrue(new Node("41000000000").IsInt);
                Assert.IsTrue(new Node("3.14159265358979323").IsFloat);
                // Assert.IsTrue(new Node(".42").IsFloat); // not on mono
                Assert.IsTrue(new Node("{}").IsDictionary);
                Assert.IsTrue(new Node("{a:41}").IsDictionary);
                Assert.IsTrue(new Node("{a:'41'}").IsDictionary);
                Assert.IsTrue(new Node("{a:41000000000}").IsDictionary);
                Assert.IsTrue(new Node("{a:3.1415927}").IsDictionary);
                Assert.IsTrue(new Node("{a:3.14159265358979323}").IsDictionary);
                Assert.IsTrue(new Node("{'a':41}").IsDictionary);
                Assert.IsTrue(new Node("{'a':'41'}").IsDictionary);
                Assert.IsTrue(new Node("{\"a\":\"41\"}").IsDictionary);
                Assert.IsTrue(new Node("[]").IsList);
                Assert.IsTrue(new Node("['a']").IsList);
                Assert.IsTrue(new Node("['a','b']").IsList);
                Assert.IsTrue(new Node("[41,42]").IsList);
                Assert.IsTrue(new Node("['a',42]").IsList);
                Assert.AreEqual(new Node("['a',42]").AsList.First().AsString, "a");
                Assert.AreEqual(new Node("['a',42]").AsList.ElementAt(1).AsInt, 42);
                Assert.IsTrue(new Node("{a:41}").AsDictionary.First().Value.IsInt);
                Assert.IsTrue(new Node("{a:'41'}").AsDictionary.First().Value.IsString);
                Assert.IsTrue(new Node("{a:41000000000}").AsDictionary.First().Value.IsInt);
                Assert.IsTrue(new Node("{a:3.1415927}").AsDictionary.First().Value.IsFloat);
                Assert.IsTrue(new Node("{a:3.14159265358979323}").AsDictionary.First().Value.IsFloat);
                Assert.IsTrue(new Node("{a:.42}").AsDictionary.First().Value.IsFloat);
                Assert.AreEqual("true", new Node("{a:true}").AsDictionary["a"].AsString);
                Assert.AreEqual("false", new Node("{a:false}").AsDictionary["a"].AsString);
                Assert.AreEqual(41, new Node("{a:41}").AsDictionary.First().Value.AsInt);
                Assert.AreEqual(41, new Node("{a:41}").AsDictionary.First().Value.AsFloat);
                Assert.AreEqual("41", new Node("{a:'41'}").AsDictionary.First().Value.AsString);
                Assert.AreEqual(41000000000, new Node("{a:41000000000}").AsDictionary.First().Value.AsInt);
                Assert.AreEqual(3.1415927, new Node("{a:3.1415927}").AsDictionary.First().Value.AsFloat);
                Assert.AreEqual(3.1415927, new Node("{a:3.1415927}").AsDictionary["a"].AsFloat);
                Assert.AreEqual(3.14159265358979323, new Node("{a:3.14159265358979323}").AsDictionary.First().Value.AsFloat);
                Assert.AreEqual("41", new Node("41").AsString);
                Assert.AreEqual("41000000000", new Node("41000000000").AsString);
                Assert.AreEqual("3.14159265358979", new Node("3.14159265358979").AsString);
                Assert.AreEqual(3.14159265358979, new Node("3.14159265358979").AsFloat);
            }

            [TestMethod]
            public void FloatFromStringWithInvariantCulture()
            {
                Assert.AreEqual(3.14159265358979323, new Node("{\"a\":\"3.14159265358979323\"}").Dictionary.First().Value.AsFloat);
            }

            [TestMethod]
            public void EmptyNodeForMissingDictionaryKey()
            {
                Assert.AreEqual(0, new Node("{a:41}").AsDictionary.Get("b").AsInt);
            }

            [TestMethod]
            public void EmptyNodeForMissingListIndex()
            {
                Assert.AreEqual(0, new Node("[41,42]").AsList.Get(2).AsInt);
            }

            [TestMethod]
            public void AlternativeNotationsForValueTypes()
            {
                // Arrange
                const string data = "{ a: '41', b: 42, c: true, d: 3.14 }";

                // Act
                var json = new Node(data);

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
            }

            [TestMethod]
            public void AlternativeNotationsForStructures()
            {
                // Arrange
                const string data = "{ a: 'b', c: [ 'c0', 'c1' ] }";
                var json = new Node(data);

                // Act, Assert
                Assert.AreEqual("c0", json.Object["c"].Array[0].AsString);
                Assert.AreEqual("c0", json.Dictionary["c"].List[0].AsString);
                Assert.AreEqual("c0", json.Object["c"].Array[0].AsString);
                Assert.AreEqual("c0", json.Dictionary["c"].List[0].AsString);
                Assert.AreEqual("c0", json["c"][0].AsString);
            }

            [TestMethod]
            public void ListEnumeration()
            {
                // Arrange
                const string data = "{ a: 'b', c: [ 'c0', 'c1' ] }";

                // Act
                var json = new Node(data);

                // Assert
                for (int i = 0; i < json["c"].Count; i++) {
                    Assert.AreEqual("c" + i, (string)json["c"][i]);
                }
            }

            [TestMethod]
            public void DictionaryEnumeration()
            {
                // Arrange
                const string data = "{ a: 'b', c: [ 'c0', 'c1' ] }";

                // Act
                var json = new Node(data);

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
            public void Linq()
            {
                // Arrange
                const string data = "{ a: 'b', c: [ 'c0', 'c1' ] }";

                // Act
                var json = new Node(data);

                // Assert
                Assert.AreEqual("b", json.Where(x => x.Key == "a").Select(x => x.Value).First());
                Assert.AreEqual("b", (from x in json where x.Key == "a" select x.Value).First());
            }

            [TestMethod]
            public void DeserializeTypicalJson()
            {
                var data = @"
[
  {
    aInt: 41,
    bBool: true,
    bLong: 42000000000,
    cString: '43',
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
                var json = new Node(data);

                // Assert
                Node lastListItem = null;
                string lastMapItemKey = null;
                Node lastMapItemValue = null;
                foreach (var item in json.List) {
                    lastListItem = item;
                }
                foreach (var pair in json.List.First().Dictionary) {
                    lastMapItemKey = pair.Key;
                    lastMapItemValue = pair.Value;
                }

                Assert.AreEqual(lastListItem.Dictionary.Count, 2);
                Assert.AreEqual(lastMapItemKey, "dFloat");
                Assert.AreEqual(lastMapItemValue.AsFloat, 3.14159265358979323);

                // Act/Assert
                Assert.AreEqual(json.List.ElementAt(0).Dictionary.ElementAt(0).Key, "aInt");
                Assert.AreEqual(json.List.ElementAt(0).Dictionary.ElementAt(0).Value.AsInt, 41);
                Assert.AreEqual(json.List.ElementAt(0).Dictionary.ElementAt(1).Key, "bBool");
                Assert.AreEqual(json.List.ElementAt(0).Dictionary.ElementAt(1).Value.AsBool, true);
                Assert.AreEqual(json.List.ElementAt(0).Dictionary.ElementAt(2).Key, "bLong");
                Assert.AreEqual(json.List.ElementAt(0).Dictionary.ElementAt(2).Value.AsInt, 42000000000);
                Assert.AreEqual(json.List.ElementAt(0).Dictionary.ElementAt(3).Key, "cString");
                Assert.AreEqual(json.List.ElementAt(0).Dictionary.ElementAt(3).Value.AsString, "43");
                Assert.AreEqual(json.List.ElementAt(0).Dictionary.ElementAt(4).Key, "dFloat");

                Assert.AreEqual(json.List.ElementAt(0).Dictionary.ElementAt(4).Value.AsFloat, 3.14159265358979323);
                Assert.AreEqual(json.List[0].Dictionary.ElementAt(4).Value.AsFloat, 3.14159265358979323);
                Assert.AreEqual(json.Array[0].Object.ElementAt(4).Value.AsFloat, 3.14159265358979323);

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
                Assert.AreEqual(json.Array[2].Object["aList"].Array[1].Object["bString"].AsString, "50");

                Assert.AreEqual(json.List.ElementAt(2).Dictionary.ElementAt(1).Key, "bMap");
                Assert.AreEqual(json.List.ElementAt(2).Dictionary.ElementAt(1).Value.Dictionary.Count, 2);
            }

            [TestMethod]
            public void UpperLowerCamelCaseIndependentKeys()
            {
                // Arrange
                const string data = "{ a: 'a', B: 'B', CdEf: 'CdEf', ghIj: 'ghIj'  }";
                var json = new Node(data);

                // Act, Assert
                Assert.AreEqual("a", json["a"].AsString);
                Assert.AreEqual("a", json["A"].AsString);
                Assert.AreEqual("B", json["b"].AsString);
                Assert.AreEqual("B", json["B"].AsString);
                Assert.AreEqual("CdEf", json["cdEf"].AsString);
                Assert.AreEqual("CdEf", json["CdEf"].AsString);
                Assert.AreEqual("ghIj", json["ghIj"].AsString);
                Assert.AreEqual("ghIj", json["GhIj"].AsString);
            }

            //[TestMethod]
            public void LowerUndescorevsUpperCamelCaseIndependentKeys()
            {
                // Arrange
                const string data = "{ a: 'a', B: 'B', CdEf: 'CdEf', gh_ij: 'gh_ij'  }";
                var json = new Node(data);

                // Act, Assert
                Assert.AreEqual("a", json["a"].AsString);
                Assert.AreEqual("a", json["A"].AsString);
                Assert.AreEqual("B", json["b"].AsString);
                Assert.AreEqual("B", json["B"].AsString);
                Assert.AreEqual("CdEf", json["cdEf"].AsString);
                Assert.AreEqual("CdEf", json["cd_ef"].AsString);
                Assert.AreEqual("gh_ij", json["GhIj"].AsString);
                Assert.AreEqual("gh_ij", json["gh_ij"].AsString);
            }
        }

        [TestClass]
        public class JsonPathSerializerTest
        {
            [TestMethod]
            public void JsonPath_serializes_simple_JSON()
            {
                // Arrange
                const string sIn = "{ a: 'a', b: 1, c: true, d: 1.11, e: [ 'e1', 'e2' ], f: { f1: 'f1', f2: 'f2' } }";
                var root = new Node(sIn);

                // Act
                string sOut = root.ToJson();

                // Assert
                Assert.IsFalse(String.IsNullOrEmpty(sOut));
            }

            [TestMethod]
            public void JsonPath_serializes_array_of_dictionary()
            {
                // Arrange
                const string sIn = "[ { 'param': { 'name': 'defaultsequence', 'value': 'idle' }}, { 'sequence': { 'group': 'idle', 'name': 'still', 'type': 'status', 'probability': '1000', 'in': 'standard', 'out': 'standard', 'src': 'idle.gif' }} ]";
                var root = new Node(sIn);

                // Act
                string sOut = root.ToJson();

                // Assert
                Assert.IsFalse(String.IsNullOrEmpty(sOut));
            }

            [TestMethod]
            public void JsonPath_Serializer_ToString_creates_human_readable_JS_with_few_double_quotes()
            {
                // Arrange
                const string sIn = "{ a: 'b\\'c b\"c', b: 1, c: true, d: false, e: 1.11, f: [ 'g', 'h' ], i: { j: 'k', l: 2, m: [ 1, 2 ], n: { o: 3, p: 4, q: { r: 's', t: [ 1, 2, 3 ], u: [ { v: 1, w: 2 }, { x: 3, y: 4 } ] } } } }";
                var root = new Node(sIn);

                // Act
                string sOut = root.ToString();

                // Assert
                Assert.AreEqual(sIn, sOut);
            }

            [TestMethod]
            public void JsonPath_Serializer_defaults_to_double_quotes_and_quoted_keys_and_no_formatting()
            {
                // Arrange
                const string sIn = "{\"a\":\"a\",\"b\":1,\"c\":true,\"d\":1.11,\"e\":[\"e1\",\"e2\"],\"f\":{\"f1\":\"f1\",\"f2\":\"f2\"}}";
                var root = new Node(sIn);

                // Act
                string sOut = root.ToJson();

                // Assert
                Assert.AreEqual(sIn, sOut);
            }

            [TestMethod]
            public void JsonPath_serializes_deserialized_data_with_added_node()
            {
                // Arrange
                const string sIn = @"
[
  {
    aInt: 41,
    bBool: true,
    cLong: 42000000000,
    dString: '43',
    eFloat: 3.14159265358979323
  },
  {
    fInt: 44,
    gLong: 45000000000,
    hString: ""46""
  },
  {
    iList:
    [
      {
        jInt: 47,
        kString: 'true'
      },
      {
        lInt: 49,
        mString: '50'
      }
    ],
    nMap:
    {
      oInt: 51,
      pString: '52'
    }
  }
]
";
                var root = new Node(sIn);

                // Act
                root.List.ElementAt(2).Dictionary.Add("new child", new Node(Node.Type.String) { Value = "new string" });

                // Assert
                string sOut = root.ToJson();
                Assert.IsTrue(sOut.Contains("\"new child\":\"new string\""));
                var check = new Node(sOut);
                Assert.AreEqual(check.List.ElementAt(2).Dictionary["new child"].String, "new string");
            }

            [TestMethod]
            public void JsonPath_serializes_and_escapes()
            {
                // Arrange
                var root = new Node(Node.Type.Dictionary);
                root.Dictionary.Add("a", "a b");
                root.Dictionary.Add("b", "a\nb");
                root.Dictionary.Add("c", "a\rb");
                root.Dictionary.Add("d", "a\\b");
                root.Dictionary.Add("e", "a\"b");
                root.Dictionary.Add("f", "a'b");
                string json = root.ToJson();

                // Act
                var node = new Node(json);

                // Assert
                Assert.AreEqual("a b", node.Dictionary["a"]);
                Assert.AreEqual("a\nb", node.Dictionary["b"]);
                Assert.AreEqual("a\rb", node.Dictionary["c"]);
                Assert.AreEqual("a\\b", node.Dictionary["d"]);
                Assert.AreEqual("a\"b", node.Dictionary["e"]);
                Assert.AreEqual("a'b", node.Dictionary["f"]);
            }
        }
    }
}
