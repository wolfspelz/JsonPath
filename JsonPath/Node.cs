using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace JsonPath
{
    public class Dictionary : Dictionary<string, Node>
    {
        public Node Get(string key)
        {
            if (ContainsKey(key)) {
                return this[key];
            }
            return new Node(Node.Type.Empty);
        }
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

    public class Node //: IEnumerable<KeyValuePair<string, Node>>, IEnumerable<Node>
    {
        public enum Type { Empty, List, Dictionary, Int, Bool, String, Float }

        public object Value;
        private readonly Type _type = Type.Empty;
        private bool _throwExceptionIfConversionFails = false;

        public bool IsEmpty { get { return _type == Type.Empty; } }
        public bool IsList { get { return _type == Type.List; } }
        public bool IsDictionary { get { return _type == Type.Dictionary; } }
        public bool IsInt { get { return _type == Type.Int; } }
        public bool IsBool { get { return _type == Type.Bool; } }
        public bool IsString { get { return _type == Type.String; } }
        public bool IsFloat { get { return _type == Type.Float; } }

        public List AsList { get { return IsList ? (List)Value : new List(); } }
        public Dictionary AsDictionary { get { return IsDictionary ? (Dictionary)Value : new Dictionary(); } }

        // Aliases
        public List List { get { return AsList; } }
        public Dictionary Dictionary { get { return AsDictionary; } }
        public List AsArray { get { return AsList; } }
        public List Array { get { return AsList; } }
        public Dictionary AsObject { get { return AsDictionary; } }
        public Dictionary Object { get { return AsDictionary; } }
        public long Int { get { return AsInt; } }
        public bool Bool { get { return AsBool; } }
        public string String { get { return AsString; } }
        public double Float { get { return AsFloat; } }
        public static implicit operator int(Node node) { return (int)node.AsInt; }
        public static implicit operator long(Node node) { return node.AsInt; }
        public static implicit operator bool(Node node) { return node.AsBool; }
        public static implicit operator string(Node node) { return node.AsString; }
        public static implicit operator double(Node node) { return node.AsFloat; }
        public static implicit operator List(Node node) { return node.AsList; }
        public static implicit operator Dictionary(Node node) { return node.AsDictionary; }
        public Node this[int index] { get { return AsList.Get(index); } }
        public Node this[string key] { get { return AsDictionary.Get(key); } }
        public Node Get(int index) { return AsList.Get(index); }
        public Node Get(string key) { return AsDictionary.Get(key); }

        // Problem: Dictionary interator seems to hide the List iterator. No way to decide which one is appropriate
        //IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<KeyValuePair<string, Node>>)AsDictionary).GetEnumerator(); }
        //IEnumerator<Node> IEnumerable<Node>.GetEnumerator() { return ((IEnumerable<Node>)AsList).GetEnumerator(); }
        //public IEnumerator<KeyValuePair<string, Node>> GetEnumerator() { return ((IEnumerable<KeyValuePair<string, Node>>)AsDictionary).GetEnumerator(); }

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
                        return "<Dictionary>";
                    } else if (IsList) {
                        return "<List>";
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
                    return (double)AsInt;
                } else if (IsString) {
                    double result = 0.0;
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

        public Node(Type type, object value = null)
        {
            _type = type;

            if (value != null) {
                switch (type) {
                    case Type.Int: Value = Convert.ToInt64(value); break;
                    case Type.Float: Value = Convert.ToDouble(value); break;
                    default: Value = value; break;
                }
            } else {
                switch (type) {
                    case Type.List: Value = new List(); break;
                    case Type.Dictionary: Value = new Dictionary(); break;
                    case Type.Int: Value = 0; break;
                    case Type.Bool: Value = false; break;
                    case Type.String: Value = ""; break;
                    case Type.Float: Value = 0.0; break;
                }
            }
        }

        public Node(string sJson, Deserializer.Options options = null)
        {
            var node = Deserializer.FromJson(sJson, options);
            _type = node._type;
            Value = node.Value;
        }
    }
}
