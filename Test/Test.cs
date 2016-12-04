using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JsonPath;
using System.Linq;

namespace Test.JsonPath
{
    [TestClass]
    public class Test
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
            const string sIn = "{ a: '41', b: 42, c: true, d: 3.14 }";

            // Act
            var root = new Node(sIn);

            // Assert
            Assert.AreEqual("41", root.AsDictionary["a"].String);
            Assert.AreEqual(42, root.AsDictionary["b"].Int);
            Assert.AreEqual(true, root.AsDictionary["c"].Bool);
            Assert.AreEqual(3.14, root.AsDictionary["d"].Float);

            Assert.AreEqual("41", (string)root.AsDictionary["a"]);
            Assert.AreEqual(42, (long)root.AsDictionary["b"]);
            Assert.AreEqual(42, (int)root.AsDictionary["b"]);
            Assert.AreEqual(true, (bool)root.AsDictionary["c"]);
            Assert.AreEqual(3.14, (double)root.AsDictionary["d"]);

            string s = root.AsDictionary["a"];
            Assert.AreEqual("41", s);
            long l = root.AsDictionary["b"];
            Assert.AreEqual(42, l);
            int i = root.AsDictionary["b"];
            Assert.AreEqual(42, i);
            bool b = root.AsDictionary["c"];
            Assert.AreEqual(true, b);
            double d = root.AsDictionary["d"];
            Assert.AreEqual(3.14, d);
        }

        [TestMethod]
        public void AlternativeNotationsForStructures()
        {
            // Arrange
            const string sIn = "{ a: 'a', b: [ 'b0', 'b1' ] }";

            // Act
            var root = new Node(sIn);

            // Assert
            Assert.AreEqual("b0", root.Object["b"].Array[0].AsString);
            Assert.AreEqual("b0", root.Dictionary["b"].List[0].AsString);
            Assert.AreEqual("b0", root.AsObject["b"].AsArray[0].AsString);
            Assert.AreEqual("b0", root.AsDictionary["b"].AsList[0].AsString);
            Assert.AreEqual("b0", root["b"][0].AsString);
        }

        //[TestMethod]
        //public void Date()
        //{
        //    Assert.AreEqual(1245398693390, new Node("/Date(1245398693390)/").Int);
        //}

        [TestMethod]
        public void FloatFromStringWithInvariantCulture()
        {
            Assert.AreEqual(3.14159265358979323, new Node("{\"a\":\"3.14159265358979323\"}").Dictionary.First().Value.AsFloat);
        }

        [TestMethod]
        public void DeserializeTypicalJson()
        {
            var sJson = @"
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
            var root = new Node(sJson);

            // Assert
            Node lastListItem = null;
            string lastMapItemKey = null;
            Node lastMapItemValue = null;
            foreach (var item in root.List) {
                lastListItem = item;
            }
            foreach (var pair in root.List.First().Dictionary) {
                lastMapItemKey = pair.Key;
                lastMapItemValue = pair.Value;
            }

            Assert.AreEqual(lastListItem.Dictionary.Count, 2);
            Assert.AreEqual(lastMapItemKey, "dFloat");
            Assert.AreEqual(lastMapItemValue.AsFloat, 3.14159265358979323);

            // Act/Assert
            Assert.AreEqual(new Node(sJson).List.ElementAt(0).Dictionary.ElementAt(0).Key, "aInt");
            Assert.AreEqual(new Node(sJson).List.ElementAt(0).Dictionary.ElementAt(0).Value.AsInt, 41);
            Assert.AreEqual(new Node(sJson).List.ElementAt(0).Dictionary.ElementAt(1).Key, "bBool");
            Assert.AreEqual(new Node(sJson).List.ElementAt(0).Dictionary.ElementAt(1).Value.AsBool, true);
            Assert.AreEqual(new Node(sJson).List.ElementAt(0).Dictionary.ElementAt(2).Key, "bLong");
            Assert.AreEqual(new Node(sJson).List.ElementAt(0).Dictionary.ElementAt(2).Value.AsInt, 42000000000);
            Assert.AreEqual(new Node(sJson).List.ElementAt(0).Dictionary.ElementAt(3).Key, "cString");
            Assert.AreEqual(new Node(sJson).List.ElementAt(0).Dictionary.ElementAt(3).Value.AsString, "43");
            Assert.AreEqual(new Node(sJson).List.ElementAt(0).Dictionary.ElementAt(4).Key, "dFloat");

            Assert.AreEqual(new Node(sJson).List.ElementAt(0).Dictionary.ElementAt(4).Value.AsFloat, 3.14159265358979323);
            Assert.AreEqual(new Node(sJson).List[0].Dictionary.ElementAt(4).Value.AsFloat, 3.14159265358979323);
            Assert.AreEqual(new Node(sJson).Array[0].Object.ElementAt(4).Value.AsFloat, 3.14159265358979323);

            Assert.AreEqual(new Node(sJson).List.ElementAt(1).Dictionary.ElementAt(0).Key, "aInt");
            Assert.AreEqual(new Node(sJson).List.ElementAt(1).Dictionary.ElementAt(0).Value.AsInt, 44);
            Assert.AreEqual(new Node(sJson).List.ElementAt(1).Dictionary.ElementAt(1).Key, "bLong");
            Assert.AreEqual(new Node(sJson).List.ElementAt(1).Dictionary.ElementAt(1).Value.AsInt, 45000000000);
            Assert.AreEqual(new Node(sJson).List.ElementAt(1).Dictionary.ElementAt(2).Key, "cString");
            Assert.AreEqual(new Node(sJson).List.ElementAt(1).Dictionary.ElementAt(2).Value.AsString, "46");

            Assert.AreEqual(new Node(sJson).List.ElementAt(2).Dictionary.ElementAt(0).Key, "aList");
            Assert.AreEqual(new Node(sJson).List.ElementAt(2).Dictionary.ElementAt(0).Value.List.Count, 2);
            Assert.AreEqual(new Node(sJson).List.ElementAt(2).Dictionary.ElementAt(0).Value.List.ElementAt(0).Dictionary.ElementAt(0).Key, "aInt");
            Assert.AreEqual(new Node(sJson).List.ElementAt(2).Dictionary.ElementAt(0).Value.List.ElementAt(0).Dictionary.ElementAt(0).Value.AsInt, 47);
            Assert.AreEqual(new Node(sJson).List.ElementAt(2).Dictionary.ElementAt(0).Value.List.ElementAt(0).Dictionary.ElementAt(1).Key, "bString");
            Assert.AreEqual(new Node(sJson).List.ElementAt(2).Dictionary.ElementAt(0).Value.List.ElementAt(0).Dictionary.ElementAt(1).Value.AsString, "48");
            Assert.AreEqual(new Node(sJson).List.ElementAt(2).Dictionary.ElementAt(0).Value.List.ElementAt(1).Dictionary.ElementAt(0).Key, "aInt");
            Assert.AreEqual(new Node(sJson).List.ElementAt(2).Dictionary.ElementAt(0).Value.List.ElementAt(1).Dictionary.ElementAt(0).Value.AsInt, 49);
            Assert.AreEqual(new Node(sJson).List.ElementAt(2).Dictionary.ElementAt(0).Value.List.ElementAt(1).Dictionary.ElementAt(1).Key, "bString");

            Assert.AreEqual(new Node(sJson).List.ElementAt(2).Dictionary.ElementAt(0).Value.List.ElementAt(1).Dictionary.ElementAt(1).Value.AsString, "50");
            Assert.AreEqual(new Node(sJson).List[2].Dictionary["aList"].List[1].Dictionary["bString"].AsString, "50");
            Assert.AreEqual(new Node(sJson).Array[2].Object["aList"].Array[1].Object["bString"].AsString, "50");

            Assert.AreEqual(new Node(sJson).List.ElementAt(2).Dictionary.ElementAt(1).Key, "bMap");
            Assert.AreEqual(new Node(sJson).List.ElementAt(2).Dictionary.ElementAt(1).Value.Dictionary.Count, 2);
        }

    }
}
