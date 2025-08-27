using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonPath.Test
{
    [TestClass]
    public class JsonPathSerializerTest
    {
        [TestMethod]
        [TestCategory("JsonPath")]
        public void Deserializes_Serializer_output()
        {
            // Arrange
            const string sIn = "{ a: 'a', b: 1, c: true, d: 1.11, e: '2017-01-02T03:04:05.678', f: [ 'f1', 'f2', 'f3' ], g: { g1: 'g1', g2: 'g2' } }";
            var root = JsonPath.Node.FromJson(sIn);

            // Act
            string sOut = root.ToJson();
            var node = JsonPath.Node.FromJson(sOut);

            // Assert
            Assert.AreEqual("a", node.AsDictionary["a"].AsString);
            Assert.AreEqual(1, node.AsDictionary["b"].AsInt);
            Assert.AreEqual(true, node.AsDictionary["c"].AsBool);
            Assert.AreEqual(1.11, node.AsDictionary["d"].AsFloat, 0.1);
            Assert.AreEqual(new System.DateTime(2017, 1, 2, 3, 4, 5, 678), node.AsDictionary["e"].AsDate);
            Assert.AreEqual(3, node.AsDictionary["f"].AsList.Count);
            Assert.AreEqual(2, node.AsDictionary["g"].AsDictionary.Count);
        }

        [TestMethod]
        [TestCategory("JsonPath")]
        public void Serializes_array_of_dictionary()
        {
            // Arrange
            const string sIn = "[ { 'param': { 'name': 'defaultsequence', 'value': 'idle' }}, { 'sequence': { 'group': 'idle', 'name': 'still', 'type': 'status', 'probability': '1000', 'in': 'standard', 'out': 'standard', 'src': 'idle.gif' }} ]";
            var root = JsonPath.Node.FromJson(sIn);

            // Act
            string sOut = root.ToJson();

            // Assert
            Assert.IsFalse(String.IsNullOrEmpty(sOut));
        }

        [TestMethod]
        [TestCategory("JsonPath")]
        public void ToJavascript_creates_human_readable_JS_with_few_double_quotes()
        {
            // Arrange
            const string sIn = "{ a: 'b\\'c b\"c', b: 1, c: true, d: false, e: 1.11, f: [ 'g', 'h' ], i: { j: 'k', l: 2, m: [ 1, 2 ], n: { o: 3, p: 4, q: { r: 's', t: [ 1, 2, 3 ], u: [ { v: 1, w: 2 }, { x: 3, y: 4 } ] } } } }";
            var root = JsonPath.Node.FromJson(sIn);

            // Act
            string sOut = root.ToJavascript(true, false);

            // Assert
            Assert.AreEqual(sIn, sOut);
        }

        [TestMethod]
        [TestCategory("JsonPath")]
        public void ToJavascript_creates_valid_JS_for_weird_keys()
        {
            // Arrange
            var root = JsonPath.Node.From(new Dictionary<string, bool> {
                ["a"]   = true,
                ["a_"]  = true,
                ["a_b"] = true,
                ["a-b"] = true,
                ["a.b"] = true,
                ["a-"]  = true,
                ["a1"]  = true,
                ["1"]   = true,
                [","]   = true,
                ["@"]   = true,
                [":"]   = true,
                ["-"]   = true,
                ["."]   = true,
            });

            // Act
            string sOut = root.ToJavascript();

            // Assert
            var check = JsonPath.Node.FromJson(sOut);
            Assert.IsTrue(check["a"]  );
            Assert.IsTrue(check["a_"] );
            Assert.IsTrue(check["a_b"]);
            Assert.IsTrue(check["a-b"]);
            Assert.IsTrue(check["a.b"]);
            Assert.IsTrue(check["a-"] );
            Assert.IsTrue(check["a1"] );
            Assert.IsTrue(check["1"]  );
            Assert.IsTrue(check[","]  );
            Assert.IsTrue(check["@"]  );
            Assert.IsTrue(check[":"]  );
            Assert.IsTrue(check["-"]  );
            Assert.IsTrue(check["."]  );
        }

        [TestMethod]
        [TestCategory("JsonPath")]
        public void IsGoodKey()
        {
            Assert.IsTrue(JsonPath.Serializer.IsGoodKey("a"));
            Assert.IsTrue(JsonPath.Serializer.IsGoodKey("abc"));
            Assert.IsTrue(JsonPath.Serializer.IsGoodKey("a_"));
            Assert.IsTrue(JsonPath.Serializer.IsGoodKey("_"));
            Assert.IsTrue(JsonPath.Serializer.IsGoodKey("A"));
            Assert.IsTrue(JsonPath.Serializer.IsGoodKey("aA"));
            Assert.IsTrue(JsonPath.Serializer.IsGoodKey("a1"));

            Assert.IsFalse(JsonPath.Serializer.IsGoodKey(""));
            Assert.IsFalse(JsonPath.Serializer.IsGoodKey("0"));
            Assert.IsFalse(JsonPath.Serializer.IsGoodKey("0a"));
            Assert.IsFalse(JsonPath.Serializer.IsGoodKey("11"));
            Assert.IsFalse(JsonPath.Serializer.IsGoodKey(":"));
            Assert.IsFalse(JsonPath.Serializer.IsGoodKey(";"));
            Assert.IsFalse(JsonPath.Serializer.IsGoodKey(","));
            Assert.IsFalse(JsonPath.Serializer.IsGoodKey("-"));
            Assert.IsFalse(JsonPath.Serializer.IsGoodKey("a-"));
            Assert.IsFalse(JsonPath.Serializer.IsGoodKey("-a"));
            Assert.IsFalse(JsonPath.Serializer.IsGoodKey("a-b"));
            Assert.IsFalse(JsonPath.Serializer.IsGoodKey("a.b"));
            Assert.IsFalse(JsonPath.Serializer.IsGoodKey("."));
            Assert.IsFalse(JsonPath.Serializer.IsGoodKey("@"));
        }

        [TestMethod]
        [TestCategory("JsonPath")]
        public void Defaults_to_double_quotes_and_quoted_keys_and_no_formatting()
        {
            // Arrange
            const string sIn = "{\"a\":\"a\",\"b\":1,\"c\":true,\"d\":1.11,\"e\":[\"e1\",\"e2\"],\"f\":{\"f1\":\"f1\",\"f2\":\"f2\"}}";
            var root = JsonPath.Node.FromJson(sIn);

            // Act
            string sOut = root.ToJson();

            // Assert
            Assert.AreEqual(sIn, sOut);
        }

        [TestMethod]
        [TestCategory("JsonPath")]
        public void Serializes_deserialized_data_with_added_node()
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
            var root = JsonPath.Node.FromJson(sIn);

            // Act
            root.List.ElementAt(2).Dictionary.Add("new child", new Node(JsonPath.Node.Type.String) { Value = "new string" });

            // Assert
            string sOut = root.ToJson();
            Assert.IsTrue(sOut.Contains("\"new child\":\"new string\""));
            var check = JsonPath.Node.FromJson(sOut);
            Assert.AreEqual(check.List.ElementAt(2).Dictionary["new child"].String, "new string");
        }

    }
}
