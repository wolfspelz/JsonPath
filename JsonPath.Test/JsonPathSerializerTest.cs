using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonPath.Test
{
    [TestClass]
    public class JsonPathSerializerTest
    {
        [TestMethod]
        public void Deserializes_Serializer_output()
        {
            // Arrange
            const string sIn = "{ a: 'a', b: 1, c: true, d: 1.11, e: '2017-01-02T03:04:05.678', f: [ 'f1', 'f2', 'f3' ], g: { g1: 'g1', g2: 'g2' } }";
            var root = new Node(sIn);

            // Act
            string sOut = root.ToJson();
            var node = new JsonPath.Node(sOut);

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
        public void Serializes_array_of_dictionary()
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
        public void ToReadableJson_creates_human_readable_JS_with_few_double_quotes()
        {
            // Arrange
            const string sIn = "{ a: 'b\\'c b\"c', b: 1, c: true, d: false, e: 1.11, f: [ 'g', 'h' ], i: { j: 'k', l: 2, m: [ 1, 2 ], n: { o: 3, p: 4, q: { r: 's', t: [ 1, 2, 3 ], u: [ { v: 1, w: 2 }, { x: 3, y: 4 } ] } } } }";
            var root = new Node(sIn);

            // Act
            string sOut = root.ToReadableJson();

            // Assert
            Assert.AreEqual(sIn, sOut);
        }

        [TestMethod]
        public void Defaults_to_double_quotes_and_quoted_keys_and_no_formatting()
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
            var root = new Node(sIn);

            // Act
            root.List.ElementAt(2).Dictionary.Add("new child", new Node(JsonPath.Node.Type.String) { Value = "new string" });

            // Assert
            string sOut = root.ToJson();
            Assert.IsTrue(sOut.Contains("\"new child\":\"new string\""));
            var check = new Node(sOut);
            Assert.AreEqual(check.List.ElementAt(2).Dictionary["new child"].String, "new string");
        }

    }
}
