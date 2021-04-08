using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;

namespace JsonPath
{
    public class SerializerOptions
    {
        public bool BlankBeforeMapColon = false;
        public bool BlankAfterMapColon = false;
        public bool BlankBeforeMapBracket = false;
        public bool BlankAfterMapBracket = false;
        public bool BlankBeforeListComma = false;
        public bool BlankAfterListComma = false;
        public bool BlankBeforeListBracket = false;
        public bool BlankAfterListBracket = false;
        public bool BlankBeforeMapComma = false;
        public bool BlankAfterMapComma = false;
        public bool WrapAfterMapPair = false;
        public bool WrapAfterListElement = false;
        public bool IndentList = false;
        public bool IndentMap = false;
        public string IndentString = "";
        public string EncapsulateKeys = "\"";
        public string EncapsulateStrings = "\"";
        public bool UseJsonNETStringSerializer = false;

        public SerializerOptions(bool bFormatted = false, bool bWrapped = false)
        {
            if (bFormatted) {
                if (bWrapped) {
                    IndentMap = true;
                    WrapAfterMapPair = true;
                    IndentList = true;
                    WrapAfterListElement = true;
                    IndentString = "  ";
                }

                BlankAfterMapColon = true;
                BlankAfterListComma = true;
                BlankAfterMapComma = true;

                BlankBeforeListBracket = true;
                BlankAfterListBracket = true;

                BlankBeforeMapBracket = true;
                BlankAfterMapBracket = true;
            }
        }

    }

    public class Serializer
    {

        private SerializerOptions _options = new SerializerOptions();

        private int IndentDepth { get; set; }

        private void Indent(StringBuilder sb)
        {
            for (int i = 0; i < IndentDepth; i++) {
                sb.Append(_options.IndentString);
            }
        }

        public static string ToJson(Node node, SerializerOptions options)
        {
            var js = new Serializer();

            if (options != null) {
                js._options = options;
            }

            return js.Serialize(node);
        }

        public static string ToJson(Node node, bool bFormatted = false, bool bWrapped = false)
        {
            var ser = new Serializer {
                _options = new SerializerOptions(bFormatted, bWrapped)
            };

            return ser.Serialize(node);
        }

        static readonly Dictionary<string, string> EscapeStringTable = new Dictionary<string, string>()
        {
            { "\\", "\\\\" },
            { "\r", "\\r" },
            { "\n", "\\n" },
            { "\t", "\\t" },
            { "\b", "\\b" },
        };
        private static string EscapeString(StringBuilder data)
        {
            foreach (var pair in EscapeStringTable) {
                data.Replace(pair.Key, pair.Value);
            }
            return data.ToString();
        }

        private string Serialize(Node node)
        {
            var sb = new StringBuilder();

            if (node.IsInt) {
                sb.Append(node.Int);
            }

            if (node.IsString) {
                if (_options.UseJsonNETStringSerializer) {
                    sb.Append(JsonConvert.ToString(node.String));
                } else {
                    sb.Append(_options.EncapsulateStrings);
                    sb.Append(EscapeString(new StringBuilder(node.String, node.String.Length * 2)).Replace(_options.EncapsulateStrings, "\\" + _options.EncapsulateStrings));
                    sb.Append(_options.EncapsulateStrings);
                }
            }

            if (node.IsFloat) {
                sb.Append(node.Float.ToString(CultureInfo.InvariantCulture));
            }

            if (node.IsDate) {
                sb.Append(_options.EncapsulateStrings);
                sb.Append(node.Date.ToString("o"));
                sb.Append(_options.EncapsulateStrings);
            }

            if (node.IsBool) {
                sb.Append(node.Bool ? "true" : "false");
            }

            if (node.IsList) {
                sb.Append("[");
                if (node.List.Count > 0) {
                    if (_options.BlankAfterListBracket) { sb.Append(" "); }
                    if (_options.IndentList) { sb.Append("\n"); IndentDepth++; }
                    bool bFirst = true;
                    foreach (var prop in node.List) {
                        if (!bFirst) {
                            if (_options.BlankBeforeListComma) { sb.Append(" "); }
                            sb.Append(",");
                            if (_options.BlankAfterListComma) { sb.Append(" "); }
                            if (_options.WrapAfterListElement) { sb.Append("\n"); }
                        }
                        bFirst = false;
                        if (_options.IndentList) { Indent(sb); }
                        sb.Append(Serialize(prop));
                    }
                    if (_options.BlankBeforeListBracket) { sb.Append(" "); }
                    if (_options.IndentList) { sb.Append("\n"); IndentDepth--; }
                    if (_options.IndentList) { Indent(sb); }
                }
                sb.Append("]");
            }

            if (node.IsDictionary) {
                sb.Append("{");
                if (node.Dictionary.Count > 0) {
                    if (_options.BlankAfterMapBracket) { sb.Append(" "); }
                    if (_options.IndentMap) { sb.Append("\n"); IndentDepth++; }
                    bool bFirst = true;
                    foreach (var prop in node.Dictionary) {
                        if (!bFirst) {
                            if (_options.BlankBeforeMapComma) { sb.Append(" "); }
                            sb.Append(",");
                            if (_options.BlankAfterMapComma) { sb.Append(" "); }
                            if (_options.WrapAfterMapPair) { sb.Append("\n"); }
                        }
                        bFirst = false;
                        if (_options.IndentMap) { Indent(sb); }
                        sb.Append(_options.EncapsulateKeys);
                        sb.Append(prop.Key);
                        sb.Append(_options.EncapsulateKeys);
                        if (_options.BlankBeforeMapColon) { sb.Append(" "); }
                        sb.Append(":");
                        if (_options.BlankAfterMapColon) { sb.Append(" "); }
                        sb.Append(Serialize(prop.Value));
                    }
                    if (_options.BlankBeforeMapBracket) { sb.Append(" "); }
                    if (_options.IndentMap) { sb.Append("\n"); IndentDepth--; }
                    if (_options.IndentMap) { Indent(sb); }
                }
                sb.Append("}");
            }

            return sb.ToString();
        }
    }
}
