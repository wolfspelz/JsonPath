using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace JsonPath
{
    public class Dictionary : Dictionary<string, Node>
    {
        public Node Get(string key)
        {
            if (ContainsKey(key)) {
                return this[key];
            }

            var lowerKey = ToLowerCamel(key);
            foreach (var prop in this.Keys) {
                if (ToLowerCamel(prop) == lowerKey) {
                    return this[prop];
                }
            }

            //var underscoreKey = ToUndeescoreLower(key);
            //foreach (var prop in this.Keys) {
            //    if (ToUndeescoreLower(prop) == lowerKey) {
            //        return this[prop];
            //    }
            //}

            return new Node(Node.Type.Empty);
        }

        private string ToLowerCamel(string s)
        {
            var converted = "";

            if (s.Length > 0) {
                converted += s.ToLower()[0];
                converted += s.Substring(1);
            }

            return converted;
        }

        public void Add(string key, string value) { base.Add(key, new Node(Node.Type.String, value)); }
        public void Add(string key, int value) { base.Add(key, new Node(Node.Type.Int, value)); }
        public void Add(string key, long value) { base.Add(key, new Node(Node.Type.Int, value)); }
        public void Add(string key, float value) { base.Add(key, new Node(Node.Type.Float, value)); }
        public void Add(string key, double value) { base.Add(key, new Node(Node.Type.Float, value)); }
        public void Add(string key, bool value) { base.Add(key, new Node(Node.Type.Bool, value)); }
        public new void Add(string key, Node node) { base.Add(key, node); }
    }

    public class List : List<Node>
    {
        public Node Get(int index)
        {
            if (index < Count) {
                return this[index];
            }
            return new Node(Node.Type.Empty);
        }
    }

    public class Node : IEnumerable<KeyValuePair<string, Node>>
    {
        public enum Type { Empty, List, Dictionary, Int, Bool, String, Float }

        public object Value;
        // ReSharper disable once MemberInitializerValueIgnored
        private readonly Type _type = Type.Empty;
        private bool _throwExceptionIfConversionFails = false;

        public bool IsEmpty => _type == Type.Empty;
        public bool IsList => _type == Type.List;
        public bool IsDictionary => _type == Type.Dictionary;
        public bool IsInt => _type == Type.Int;
        public bool IsBool => _type == Type.Bool;
        public bool IsString => _type == Type.String;
        public bool IsFloat => _type == Type.Float;

        public List AsList => IsList ? (List)Value : new List();
        public Dictionary AsDictionary => IsDictionary ? (Dictionary)Value : new Dictionary();

        // Aliases
        public List List => AsList;
        public Dictionary Dictionary => AsDictionary;
        public List AsArray => AsList;
        public List Array => AsList;
        public Dictionary AsObject => AsDictionary;
        public Dictionary Object => AsDictionary;
        public long Int => AsInt;
        public bool Bool => AsBool;
        public string String => AsString;
        public double Float => AsFloat;
        public static implicit operator int(Node node) { return (int)node.AsInt; }
        public static implicit operator long(Node node) { return node.AsInt; }
        public static implicit operator bool(Node node) { return node.AsBool; }
        public static implicit operator string(Node node) { return node.AsString; }
        public static implicit operator double(Node node) { return node.AsFloat; }
        public static implicit operator List(Node node) { return node.AsList; }
        public static implicit operator Dictionary(Node node) { return node.AsDictionary; }
        public Node this[int index] => AsList.Get(index);
        public Node this[string key] => AsDictionary.Get(key);
        public Node Get(int index) { return AsList.Get(index); }
        public Node Get(string key) { return AsDictionary.Get(key); }

        // Iteration
        public IEnumerator<KeyValuePair<string, Node>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, Node>>)AsDictionary).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, Node>>)AsDictionary).GetEnumerator();
        }
        public long Count
        {
            get {
                if (IsDictionary) { return AsDictionary.Count; }
                if (IsList) { return AsList.Count; }
                return 0;
            }
        }

        public long AsInt
        {
            get {
                if (IsInt) {
                    return (long)Value;
                } else if (IsString) {
                    long result;
                    if (Int64.TryParse(AsString, out result)) {
                        return result;
                    }
                } else if (IsFloat) {
                    return Convert.ToInt64(AsFloat);
                }
                if (_throwExceptionIfConversionFails) {
                    throw new Exception("Wrong node type: trying to read " + Type.Int.ToString() + " from " + _type.ToString());
                } else {
                    return 0;
                }
            }
        }

        public bool AsBool
        {
            get {
                if (IsBool) {
                    return (bool)Value;
                } else if (IsString) {
                    var s = AsString.ToLower();
                    return s == "true";
                } else if (IsInt) {
                    return AsInt != 0;
                } else if (IsFloat) {
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    return AsFloat != 0.0;
                }
                if (_throwExceptionIfConversionFails) {
                    throw new Exception("Wrong node type: trying to read " + Type.Bool.ToString() + " from " + _type.ToString());
                } else {
                    return false;
                }
            }
        }

        public string AsString
        {
            get {
                if (IsString) {
                    return (string)Value;
                } else if (IsInt) {
                    return AsInt.ToString(CultureInfo.InvariantCulture);
                } else if (IsFloat) {
                    return String.Format(CultureInfo.InvariantCulture, "{0}", (double)Value);
                } else if (IsBool) {
                    return AsBool ? "true" : "false";
                }
                if (_throwExceptionIfConversionFails) {
                    throw new Exception("Wrong node type: trying to read " + Type.String.ToString() + " from " + _type.ToString());
                } else {
                    if (IsDictionary) {
                        return "<JSObject>";
                    } else if (IsList) {
                        return "<JSArray>";
                    }
                    return "";
                }
            }
        }

        public double AsFloat
        {
            get {
                if (IsFloat) {
                    return (double)Value;
                } else if (IsInt) {
                    return AsInt;
                } else if (IsString) {
                    double result;
                    if (Double.TryParse(AsString, NumberStyles.Any, CultureInfo.InvariantCulture, out result)) {
                        return Double.Parse(AsString, CultureInfo.InvariantCulture);
                    }
                }
                if (_throwExceptionIfConversionFails) {
                    throw new Exception("Wrong node type: trying to read " + Type.Float.ToString() + " from " + _type.ToString());
                } else {
                    return 0.0;
                }
            }
        }

        public string Key { get; set; }

        public Node(Type type, object value = null)
        {
            _type = type;

            if (value != null) {
                switch (type) {
                case Type.Int:
                    Value = Convert.ToInt64(value);
                    break;
                case Type.Float:
                    Value = Convert.ToDouble(value);
                    break;
                default:
                    Value = value;
                    break;
                }
            } else {
                switch (type) {
                case Type.List:
                    Value = new List();
                    break;
                case Type.Dictionary:
                    Value = new Dictionary();
                    break;
                case Type.Int:
                    Value = 0;
                    break;
                case Type.Bool:
                    Value = false;
                    break;
                case Type.String:
                    Value = "";
                    break;
                case Type.Float:
                    Value = 0.0;
                    break;
                }
            }
        }

        public Node(string sJson, Deserializer.Options options = null)
        {
            var node = Deserializer.FromJson(sJson, options);
            _type = node._type;
            Value = node.Value;
        }

        public string ToJson(Serializer.Options options)
        {
            return Serializer.ToJson(this, options);
        }

        public string ToJson(bool bFormatted = false, bool bWrapped = false)
        {
            return Serializer.ToJson(this, bFormatted, bWrapped);
        }

        public override string ToString()
        {
            var options = new Serializer.Options(bFormatted: true) { EncapsulateKeys = "", EncapsulateStrings = "'" };
            return Serializer.ToJson(this, options);
        }
    }
}
