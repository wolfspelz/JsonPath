using System;
using Newtonsoft.Json.Linq;

namespace JsonPath
{
    public class Deserializer
    {
        public class Options
        {
        }

        public static Node FromJson(string json, Deserializer.Options options = null)
        {
            if (string.IsNullOrEmpty(json)) {
                return new Node(Node.Type.Empty);
            }

            var jsonObject = JToken.Parse(json);
            return Deserializer.NodeFromJsonObject(jsonObject);
        }

        internal static Node NodeFromJsonObject(object obj)
        {
            if (obj == null) {
                return new Node(Node.Type.Empty);
            }

            var value = obj as JValue;
            if (value != null) {
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
                    case JTokenType.None:
                    case JTokenType.Object:
                    case JTokenType.Array:
                    case JTokenType.Constructor:
                    case JTokenType.Property:
                    case JTokenType.Date:
                    case JTokenType.Raw:
                    case JTokenType.Bytes:
                    case JTokenType.TimeSpan:
                    default:
                        throw new Exception("Json.NET JToken.Type=" + value.Type.ToString() + " not supported");
                }
            }

            var list = obj as JArray;
            if (list != null) {
                var node = new Node(Node.Type.List);
                foreach (var item in list) {
                    node.List.Add(NodeFromJsonObject(item));
                }
                return node;
            }

            var dict = obj as JObject;
            if (dict != null) {
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
