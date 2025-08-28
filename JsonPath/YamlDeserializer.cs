using System.Dynamic;
using System.Globalization;

namespace JsonPath
{
    public static class YamlDeserializer
    {
        private static string TruncateYamlBlob(string text, int nLen, string tail = "...")
        {
            if (text.Length > nLen) {
                int nMax = nLen - tail.Length;
                nMax = nMax < 0 ? 0 : nMax;
                text = text.Substring(0, nMax) + tail;
            }
            return text.Replace("\r\n", "\n").Replace("\n", "\\n").Replace("'", "\"");
        }

        public class Options
        {
            public bool LowerCaseDictKeys = false;
            public Log Log = new();
            public int LogBlobLen = 80;
            public bool LogExceptionOnParserError = true;
            public bool ThrowExceptionOnParserError = true;
        }

        public static Node Deserialize(string yaml, Options? options = null)
        {
            options ??= new Options();

            if (string.IsNullOrEmpty(yaml)) {
                return new Node(Node.Type.Empty);
            }

            try {
                var deserializer = new YamlDotNet.Serialization.DeserializerBuilder().Build();
                dynamic parsed = deserializer.Deserialize<ExpandoObject>(yaml);
                return NodeFromDynamic(parsed, options);
            } catch (YamlDotNet.Core.SemanticErrorException ex) {
                if (options.LogExceptionOnParserError) {
                    options.Log.Error($"Execption: {ex.Message} start=({ex.Start}) end=({ex.End}): offending='{yaml[ex.Start.Index..ex.End.Index]}' yaml={TruncateYamlBlob(yaml, options.LogBlobLen)}");
                }
                if (options.ThrowExceptionOnParserError) {
                    throw;
                }
                return new Node(Node.Type.Empty);
            }
        }

        private static Node NodeFromDynamic(dynamic obj, Options options)
        {
            if (obj == null) {
                return new Node(Node.Type.Empty);
            }

            if (obj is System.Dynamic.ExpandoObject expando) {
                var node = new Node(Node.Type.Dictionary);
                foreach (var pair in expando) {
                    var mappedKey = options.LowerCaseDictKeys ? pair.Key.ToLower() : pair.Key;
                    if (pair.Value != null) {
                        node.Dictionary.Add(mappedKey, NodeFromDynamic(pair.Value, options));
                    }
                }
                return node;
            }

            if (obj is string stringValue) {
                return Node.From(stringValue == null ? "" : stringValue);
            } else if (obj is int intValue) {
                return Node.From(intValue);
            } else if (obj is long longValue) {
                return Node.From(longValue);
            } else if (obj is float floatValue) {
                return Node.From(floatValue);
            } else if (obj is double doubleValue) {
                return Node.From(doubleValue);
            } else if (obj is bool boolValue) {
                return Node.From(boolValue);
            } else if (obj is DateTime dateValue) {
                return Node.From(dateValue);
            } else if (obj is Dictionary<object, object> dict) {
                var node = new Node(Node.Type.Dictionary);
                foreach (var pair in dict) {
                    var key = (string)pair.Key;
                    var mappedKey = options.LowerCaseDictKeys ? key.ToLower() : key;
                    node.Dictionary.Add(mappedKey, NodeFromDynamic(pair.Value, options));
                }
                return node;
            } else if (obj is List<object> list) {
                var node = new Node(Node.Type.List);
                foreach (var item in list) {
                    node.List.Add(NodeFromDynamic(item, options));
                }
                return node;
            } else {
                var t = obj.GetType();
                var s = t;
            }

            return new Node(Node.Type.Empty);
        }
    }
}
