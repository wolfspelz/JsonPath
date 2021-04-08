using System;
using Newtonsoft.Json.Linq;

namespace JsonPath
{
    public class DeserializerOptions
    {
    }

    public static class Deserializer
    {
        public static Action Dont { get; set; }

        public static Node FromJson(string json, DeserializerOptions options = null)
        {
            Dont = () => { var x = options; };

            if (string.IsNullOrEmpty(json)) {
                return new Node(Node.Type.Empty);
            }

            var jsonObject = JToken.Parse(json);
            return NodeFromJsonObject(jsonObject);
        }

        private static Node NodeFromJsonObject(object obj)
        {
            if (obj == null) {
                return new Node(Node.Type.Empty);
            }

            if (obj is JValue value) {
                switch (value.Type) {
                    case JTokenType.Comment: return new Node(Node.Type.Empty);
                    case JTokenType.Integer: return new Node(Node.Type.Int) { Value = (long)value.Value };
                    case JTokenType.Float: return new Node(Node.Type.Float) { Value = (double)value.Value };
                    case JTokenType.String: return new Node(Node.Type.String) { Value = (string)value.Value };
                    case JTokenType.Boolean: return new Node(Node.Type.Bool) { Value = (bool)value.Value };
                    case JTokenType.Null: return new Node(Node.Type.Empty);
                    case JTokenType.Undefined: return new Node(Node.Type.Empty);
                    case JTokenType.Guid: return new Node(Node.Type.String) { Value = value.Value.ToString() };
                    case JTokenType.Uri: return new Node(Node.Type.String) { Value = value.Value.ToString() };
                    case JTokenType.Date: return new Node(Node.Type.Date) { Value = (DateTime)value.Value };
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
                        throw new Exception("Json.NET JToken.Type=" + value.Type.ToString() + " not supported");
                }
            }

            if (obj is JArray list) {
                var node = new Node(Node.Type.List);
                foreach (var item in list) {
                    node.List.Add(NodeFromJsonObject(item));
                }
                return node;
            }

            if (obj is JObject dict) {
                var node = new Node(Node.Type.Dictionary);
                foreach (var pair in dict) {
                    node.Dictionary.Add(pair.Key, NodeFromJsonObject(pair.Value));
                }
                return node;
            }

            return new Node(Node.Type.Empty);
        }
    }
}
