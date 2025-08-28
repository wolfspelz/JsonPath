using System;
using Newtonsoft.Json.Linq;

namespace JsonPath
{
    public class DeserializerOptions
    {
        public bool LowerCaseDictKeys = false;
    }

    public static class Deserializer
    {
        public static Action? Dont { get; set; }

        public static Node FromJson(string json, DeserializerOptions? options = null)
        {
            if (string.IsNullOrEmpty(json)) {
                return new Node(Node.Type.Empty);
            }

            var jsonObject = JToken.Parse(json);
            return NodeFromJsonObject(jsonObject, options ?? new DeserializerOptions());
        }

        private static Node NodeFromJsonObject(object obj, DeserializerOptions options)
        {
            if (obj == null) {
                return new Node(Node.Type.Empty);
            }

            if (obj is JValue jv) {
                switch (jv.Type) {
                    case JTokenType.Comment: return new Node(Node.Type.Empty);
                    case JTokenType.Integer: return Node.From(jv.Value == null ? 0L : (long)jv.Value);
                    case JTokenType.Float: return Node.From(jv.Value == null ? 0.0D : (double)jv.Value );
                    case JTokenType.String: return Node.From(jv.Value == null ? "" : (string)jv.Value );
                    case JTokenType.Boolean: return Node.From(jv.Value == null ? false : (bool)jv.Value );
                    case JTokenType.Null: return new Node(Node.Type.Empty);
                    case JTokenType.Undefined: return new Node(Node.Type.Empty);
                    case JTokenType.Guid: return Node.From(jv.Value == null ? "" : (string)jv.Value);
                    case JTokenType.Uri: return Node.From(jv.Value == null ? "" : (string)jv.Value);
                    case JTokenType.Date: return Node.From(jv.Value == null ? DateTime.MinValue : (DateTime)jv.Value );
                    // ReSharper disable once RedundantCaseLabel
                    case JTokenType.None:
                    // ReSharper disable once RedundantCaseLabel
                    case JTokenType.Object:
                    // ReSharper disable once RedundantCaseLabel
                    case JTokenType.Array:
                    // ReSharper disable once RedundantCaseLabel
                    case JTokenType.Constructor:
                    // ReSharper disable once RedundantCaseLabel
                    case JTokenType.Property:
                    // ReSharper disable once RedundantCaseLabel
                    case JTokenType.Raw:
                    // ReSharper disable once RedundantCaseLabel
                    case JTokenType.Bytes:
                    // ReSharper disable once RedundantCaseLabel
                    case JTokenType.TimeSpan:
                    default:
                        throw new Exception("Json.NET JToken.Type=" + jv.Type.ToString() + " not supported");
                }
            }

            if (obj is JArray list) {
                var node = new Node(Node.Type.List);
                foreach (var item in list) {
                    node.List.Add(NodeFromJsonObject(item, options));
                }
                return node;
            }

            if (obj is JObject dict) {
                var node = new Node(Node.Type.Dictionary);
                foreach (var pair in dict) {
                    var key = options.LowerCaseDictKeys ? pair.Key.ToLower() : pair.Key;
                    node.Dictionary.Add(key, NodeFromJsonObject(pair.Value ?? JToken.Parse("''"), options));
                }
                return node;
            }

            return new Node(Node.Type.Empty);
        }
    }
}
