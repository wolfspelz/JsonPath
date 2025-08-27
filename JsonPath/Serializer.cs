using System.Globalization;
using System.Text;

using Newtonsoft.Json;

namespace JsonPath
{
    public class SerializerOptions
    {
        public string BeforeMapColon = "";
        public string AfterMapColon = "";
        public string BeforeMapBracket = "";
        public string AfterMapBracket = "";
        public string BeforeListComma = "";
        public string AfterListComma = "";
        public string BeforeListBracket = "";
        public string AfterListBracket = "";
        public string BeforeMapComma = "";
        public string AfterMapComma = "";
        public bool WrapAfterMapPair = false;
        public bool WrapAfterListElement = false;
        public bool IndentList = false;
        public bool IndentMap = false;
        public string Indent = "";
        public string EncapsulateKeys = "\"";
        public string EncapsulateBadKeys = "\"";
        public string EncapsulateStrings = "\"";
        public bool UseJsonNETStringSerializer = false;

        public SerializerOptions(bool spaced = false, bool indented = false, bool doubleQuotes = true)
        {
            if (spaced) {
                if (indented) {
                    Indent = "  ";
                    IndentMap = true;
                    WrapAfterMapPair = true;
                    IndentList = true;
                    WrapAfterListElement = true;
                }

                AfterMapColon = " ";
                AfterListComma = " ";
                AfterMapComma = " ";
                BeforeListBracket = " ";
                AfterListBracket = " ";
                BeforeMapBracket = " ";
                AfterMapBracket = " ";
            }

            if (doubleQuotes) {
                EncapsulateKeys = "\"";
                EncapsulateBadKeys = "\"";
                EncapsulateStrings = "\"";
            } else {
                EncapsulateKeys = "'";
                EncapsulateBadKeys = "'";
                EncapsulateStrings = "'";
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
                sb.Append(_options.Indent);
            }
        }

        public static string ToJson(Node node, SerializerOptions options)
        {
            var js = new Serializer { _options = options };
            return js.Serialize(node, false);
        }

        public static string ToJson(Node node, bool spaced = false, bool indented = false)
        {
            return ToJson(node, new SerializerOptions(spaced, indented));
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

        const string goodKeyChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
        const string goodKeyLeadChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_";

        public static bool IsGoodKey(string key)
        {
            if (string.IsNullOrEmpty(key)) return false;
            if (!goodKeyLeadChars.Contains(key[0])) return false;
            var len = key.Length;
            for (var i = 1; i < len; i++) {
                if (!goodKeyChars.Contains(key[i])) return false;
            }
            return true;
        }

        private string Serialize(Node node, bool isElement)
        {
            var sb = new StringBuilder();

            if (node.IsNull && isElement) {
                sb.Append("null");
            }

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
                    sb.Append(_options.AfterListBracket);
                    if (_options.IndentList) { sb.Append("\n"); IndentDepth++; }
                    bool bFirst = true;
                    foreach (var prop in node.List) {
                        if (!bFirst) {
                            sb.Append(_options.BeforeListComma);
                            sb.Append(",");
                            sb.Append(_options.AfterListComma);
                            if (_options.WrapAfterListElement) { sb.Append("\n"); }
                        }
                        bFirst = false;
                        if (_options.IndentList) { Indent(sb); }
                        sb.Append(Serialize(prop, true));
                    }
                    sb.Append(_options.BeforeListBracket);
                    if (_options.IndentList) { sb.Append("\n"); IndentDepth--; }
                    if (_options.IndentList) { Indent(sb); }
                }
                sb.Append("]");
            }

            if (node.IsDictionary) {
                sb.Append("{");
                if (node.Dictionary.Count > 0) {
                    sb.Append(_options.AfterMapBracket);
                    if (_options.IndentMap) { sb.Append("\n"); IndentDepth++; }
                    bool bFirst = true;
                    foreach (var prop in node.Dictionary) {
                        if (!bFirst) {
                            sb.Append(_options.BeforeMapComma);
                            sb.Append(",");
                            sb.Append(_options.AfterMapComma);
                            if (_options.WrapAfterMapPair) { sb.Append("\n"); }
                        }
                        bFirst = false;
                        if (_options.IndentMap) { Indent(sb); }
                        var needEncapsulateKey = !IsGoodKey(prop.Key);
                        if (needEncapsulateKey) {
                            sb.Append(_options.EncapsulateBadKeys);
                        } else {
                            sb.Append(_options.EncapsulateKeys);
                        }
                        sb.Append(prop.Key);
                        if (needEncapsulateKey) {
                            sb.Append(_options.EncapsulateBadKeys);
                        } else {
                            sb.Append(_options.EncapsulateKeys);
                        }
                        sb.Append(_options.BeforeMapColon);
                        sb.Append(":");
                        sb.Append(_options.AfterMapColon);
                        sb.Append(Serialize(prop.Value, true));
                    }
                    sb.Append(_options.BeforeMapBracket);
                    if (_options.IndentMap) { sb.Append("\n"); IndentDepth--; }
                    if (_options.IndentMap) { Indent(sb); }
                }
                sb.Append("}");
            }

            return sb.ToString();
        }
    }
}
