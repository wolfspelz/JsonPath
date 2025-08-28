using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace JsonPath.Test
{
    [TestClass]
    public class JsonPathNodeYamlTest
    {
        [TestMethod]
        [TestCategory("JsonPath")]
        public void Deserializes_YAML()
        {
            // Arrange

            const string data = @"
a: b
c: 'd'
e: 3.14159267353979323
f: 42
g: 42000000000
h: true
i:
    - j
    - k
l:
    m: n
    o: p
q: |
    r s
    t u
v: [w, x]
";

            // Act
            var n = Node.FromYaml(data);

            // Assert
            Assert.AreEqual("b", (string)n["a"]);
            Assert.AreEqual("d", (string)n["c"]);
            Assert.AreEqual(3.14159267353979323, (double)n["e"], 0.001);
            Assert.AreEqual(42, (double)n["f"]);
            Assert.AreEqual(42000000000, (long)n["g"]);
            Assert.AreEqual(true, (bool)n["h"]);
            Assert.AreEqual("j", (string)n["i"][0]);
            Assert.AreEqual("k", (string)n["i"][1]);
            Assert.AreEqual("n", (string)n["l"]["m"]);
            Assert.AreEqual("p", (string)n["l"]["o"]);
            Assert.AreEqual("r s\nt u\n", (string)n["q"]);
            Assert.AreEqual("w", (string)n["v"][0]);
            Assert.AreEqual("x", (string)n["v"][1]);

            Assert.AreEqual("b", n["a"].AsString);
            Assert.AreEqual("d", n["c"].AsString);
            Assert.AreEqual(3.14159267353979323, n["e"].AsFloat, 0.001);
            Assert.AreEqual(42, n["f"].AsFloat);
            Assert.AreEqual(42000000000, n["g"].AsInt);
            Assert.AreEqual(true, n["h"].AsBool);
            Assert.AreEqual("j", n["i"][0].AsString);
            Assert.AreEqual("k", n["i"][1].AsString);
            Assert.AreEqual("n", n["l"]["m"].AsString);
            Assert.AreEqual("p", n["l"]["o"].AsString);
            Assert.AreEqual("r s\nt u\n", n["q"].AsString);
            Assert.AreEqual("w", n["v"][0].AsString);
            Assert.AreEqual("x", n["v"][1].AsString);
        }

        [TestMethod]
        [TestCategory("JsonPath")]
        public void Deserializes_YAML_with_lower_case_keys_in_dicts()
        {
            // Arrange

            const string data = @"
lowercase: a
camelCase: b
PascalCase: c
UPPERCASE: d
indented:
    lowercase: a
    camelCase: b
    PascalCase: c
    UPPERCASE: d
";

            // Act
            var n = Node.FromYaml(data, new YamlDeserializer.Options { LowerCaseDictKeys = true });

            // Assert
            Assert.AreEqual("a", (string)n["lowercase"]);
            Assert.AreEqual("b", (string)n["camelcase"]);
            Assert.AreEqual("c", (string)n["pascalcase"]);
            Assert.AreEqual("d", (string)n["uppercase"]);

            Assert.AreEqual("a", (string)n["indented"]["lowercase"]);
            Assert.AreEqual("b", (string)n["indented"]["camelcase"]);
            Assert.AreEqual("c", (string)n["indented"]["pascalcase"]);
            Assert.AreEqual("d", (string)n["indented"]["uppercase"]);
        }

    }
}
