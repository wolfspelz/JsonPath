using System;
using Newtonsoft.Json.Linq;

// ReSharper disable once CheckNamespace
namespace JsonPath
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Deserializer
    {
        // ReSharper disable once ClassNeverInstantiated.Global
        public class Options
        {
        }

        // ReSharper disable once UnusedParameter.Global
        public static Node FromJson(string json, Options options = null)
        {
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
                case JTokenType.Date:
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
