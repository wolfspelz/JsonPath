using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace JsonPath
{
    public class Dictionary : Dictionary<string, Node>
    {
        public Node Get(string key)
        {
            if (ContainsKey(key)) {
                return base[key];
            }
            return new Node(Node.Type.Empty);
        }

        public Dictionary Add(string key, string value) { base.Add(key, new Node(Node.Type.String, value)); return this; }
        public Dictionary Add(string key, int value) { base.Add(key, new Node(Node.Type.Int, value)); return this; }
        public Dictionary Add(string key, long value) { base.Add(key, new Node(Node.Type.Int, value)); return this; }
        public Dictionary Add(string key, float value) { base.Add(key, new Node(Node.Type.Float, value)); return this; }
        public Dictionary Add(string key, double value) { base.Add(key, new Node(Node.Type.Float, value)); return this; }
        public Dictionary Add(string key, bool value) { base.Add(key, new Node(Node.Type.Bool, value)); return this; }
        public Dictionary Add(string key, DateTime value) { base.Add(key, new Node(Node.Type.Date, value)); return this; }
        public new void Add(string key, Node node) { base.Add(key, node); }
        public new Node this[string key] { get { return Get(key); } set { base[key] = value; } }
        public JsonPath.Node ToNode() { return new Node(Node.Type.Dictionary, this); }
        public static implicit operator Node(Dictionary d) { return d.ToNode(); }
    }

    public class List : List<Node>
    {
        public Node Get(int index)
        {
            if (index < Count) {
                return base[index];
            }
            return new Node(Node.Type.Empty);
        }

        public void Add(string value) { base.Add(new Node(Node.Type.String, value)); }
        public void Add(int value) { base.Add(new Node(Node.Type.Int, value)); }
        public void Add(long value) { base.Add(new Node(Node.Type.Int, value)); }
        public void Add(float value) { base.Add(new Node(Node.Type.Float, value)); }
        public void Add(double value) { base.Add(new Node(Node.Type.Float, value)); }
        public void Add(bool value) { base.Add(new Node(Node.Type.Bool, value)); }
        public void Add(DateTime value) { base.Add(new Node(Node.Type.Date, value)); }
        public void Add(object value) { base.Add(new Node(Node.Type.Auto, value)); }
        public new void Add(Node node) { base.Add(node); }
        //public new Node this[int index] { get => index < Count ? base[index] : new Node(Node.Type.Empty); set => base[index] = value; }
        public Node ToNode() { return new Node(Node.Type.List, this); }
        public static implicit operator Node(List l) { return l.ToNode(); }
    }

    public class Node : IEnumerable<KeyValuePair<string, Node>>
    {
        public enum Type { Empty, List, Dictionary, Int, Bool, String, Float, Date, Auto }

        public object Value;
        // ReSharper disable once MemberInitializerValueIgnored
        private readonly Type _type = Type.Empty;
        private readonly bool _throwExceptionIfConversionFails = false;

        public bool IsEmpty => _type == Type.Empty;
        public bool IsList => _type == Type.List;
        public bool IsDictionary => _type == Type.Dictionary;
        public bool IsInt => _type == Type.Int;
        public bool IsBool => _type == Type.Bool;
        public bool IsString => _type == Type.String;
        public bool IsFloat => _type == Type.Float;
        public bool IsDate => _type == Type.Date;
        public bool IsNull => Value == null;

        public List AsList => IsList ? (List)Value : new List();
        public Dictionary AsDictionary => IsDictionary ? (Dictionary)Value : new Dictionary();

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
        public DateTime Date => AsDate;

        public static implicit operator int(Node node) { return (int)node.AsInt; }
        public static implicit operator long(Node node) { return node.AsInt; }
        public static implicit operator bool(Node node) { return node.AsBool; }
        public static implicit operator string(Node node) { return node.AsString; }
        public static implicit operator double(Node node) { return node.AsFloat; }
        public static implicit operator DateTime(Node node) { return node.AsDate; }
        public static implicit operator List(Node node) { return node.AsList; }
        public static implicit operator Dictionary(Node node) { return node.AsDictionary; }

        public static implicit operator Node(string value) { return new Node(Type.String, value); }
        public static implicit operator Node(int value) { return new Node(Type.Int, value); }
        public static implicit operator Node(long value) { return new Node(Type.Int, value); }
        public static implicit operator Node(bool value) { return new Node(Type.Bool, value); }
        public static implicit operator Node(float value) { return new Node(Type.Float, value); }
        public static implicit operator Node(double value) { return new Node(Type.Float, value); }
        public static implicit operator Node(DateTime value) { return new Node(Type.Date, value); }

        public Node this[int index] { get { return AsList.Get(index); } set { AsList[index] = value; } }
        public Node this[string key] { get { return AsDictionary.Get(key); } set { AsDictionary[key] = value; } }
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
                    if (Int64.TryParse(AsString, out long result)) {
                        return result;
                    }
                } else if (IsFloat) {
                    return Convert.ToInt64(AsFloat);
                } else if (IsDate) {
                    return (((DateTime)Value) - new DateTime(1970, 1, 1, 0, 0, 0)).Ticks / TimeSpan.TicksPerSecond;
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
                } else if (IsDate) {
                    return ((DateTime)Value).ToString("o");
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
                } else if (IsDate) {
                    return (((DateTime)Value) - new DateTime(1899, 12, 30, 0, 0, 0, 0)).TotalDays;
                } else if (IsString) {
                    if (Double.TryParse(AsString, NumberStyles.Any, CultureInfo.InvariantCulture, out _)) {
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

        public DateTime AsDate
        {
            get {
                if (IsDate) {
                    return (DateTime)Value;
                } else if (IsFloat) {
                    return new DateTime(1899, 12, 30, 0, 0, 0, 0).AddDays(AsFloat);
                } else if (IsInt) {
                    return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(AsInt);
                } else if (IsString) {
                    if (DateTime.TryParse(AsString, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result)) {
                        // fallthru to 0-Date
                    }
                    return result;
                }
                if (_throwExceptionIfConversionFails) {
                    throw new Exception("Wrong node type: trying to read " + Type.Float.ToString() + " from " + _type.ToString());
                } else {
                    return new DateTime();
                }
            }
        }

        public string Key { get; set; }

        public Node(Type type, object value = null)
        {
            if (value is Node n) { type = n._type; value = n.Value; }

            if (type == Type.Auto) {
                if (value is string) {
                    type = Type.String;
                } else if (value is int || value is long) {
                    type = Type.Int;
                } else if (value is float || value is double) {
                    type = Type.Float;
                } else if (value is bool) {
                    type = Type.Bool;
                } else if (value is DateTime) {
                    type = Type.Date;
                }
            }

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
                    case Type.Date:
                        Value = new DateTime();
                        break;
                }
            }
        }

        public Node(string sJson, DeserializerOptions options = null)
        {
            var node = Deserializer.FromJson(sJson, options);
            _type = node._type;
            Value = node.Value;
        }

        public Node(Dictionary<string, string> dict)
        {
            _type = Type.Dictionary;
            Value = new Dictionary();

            foreach (var pair in dict) {
                AsDictionary.Add(pair.Key, new Node(Type.String, pair.Value));
            }
        }

        public Node(Dictionary<string, object> dict)
        {
            _type = Type.Dictionary;
            Value = new Dictionary();

            foreach (var pair in dict) {
                AsDictionary.Add(pair.Key, new Node(Type.Auto, pair.Value));
            }
        }

        public Node(List<string> list)
        {
            _type = Type.List;
            Value = new List();

            foreach (var item in list) {
                AsList.Add(new Node(Type.String, item));
            }
        }

        public Node(List<long> list)
        {
            _type = Type.List;
            Value = new List();

            foreach (var item in list) {
                AsList.Add(new Node(Type.Int, item));
            }
        }

        public string ToJson(SerializerOptions options)
        {
            return Serializer.ToJson(this, options);
        }

        public string ToJson(bool bFormatted = false, bool bWrapped = false)
        {
            return Serializer.ToJson(this, bFormatted, bWrapped);
        }

        public string ToReadableJson()
        {
            var options = new SerializerOptions(bFormatted: true) { EncapsulateKeys = "", EncapsulateStrings = "'" };
            return Serializer.ToJson(this, options);
        }

        public override string ToString()
        {
            switch (_type) {
                case Type.Int:
                case Type.Bool:
                case Type.String:
                case Type.Float:
                case Type.Date:
                    return AsString;
            }

            return Serializer.ToJson(this, new SerializerOptions());
        }

        public static class PatchAction
        {
            public const string Delete = "delete";
            public const string Replace = "replace";
            public const string Set = "set";
            public const string Add = "add";
            public const string Remove = "remove";
            public const string Descend = "descend";
        }

        public Node Patch(Node patch)
        {
            var node = this;

            foreach (var pair in patch.AsDictionary) {
                string action;
                string key;
                Node target;

                var parts = pair.Key.Split(new[] { '|', ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 1) {
                    key = parts[0];
                    target = node[key];
                    action = parts[parts.Length - 1].ToLower();
                } else {
                    key = pair.Key;
                    target = node[key];
                    action = target.IsDictionary ? PatchAction.Descend : PatchAction.Set;
                }

                switch (action) {
                    case PatchAction.Replace:
                    case PatchAction.Set:
                        node[key] = pair.Value;
                        break;
                    case PatchAction.Delete:
                        if (node.IsDictionary) {
                            node.AsDictionary.Remove(key);
                        }
                        break;
                    case PatchAction.Add:
                        if (target.IsList) {
                            foreach (var item in pair.Value.AsList) {
                                target.AsList.Add(item);
                            }
                        }
                        break;
                    case PatchAction.Remove:
                        if (target.IsList) {
                            foreach (var toRemove in pair.Value.AsList) {
                                foreach (var item in target.AsList) {
                                    if (toRemove.Value.Equals(item.Value)) {
                                        target.AsList.Remove(item);
                                        break;
                                    }
                                }
                            }
                        }
                        if (target.IsDictionary) {
                            foreach (var toRemove in pair.Value.AsList) {
                                foreach (var itemPair in target.AsDictionary) {
                                    if (itemPair.Key == toRemove) {
                                        target.AsDictionary.Remove(itemPair.Key);
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                    case PatchAction.Descend:
                        if (target.IsDictionary) {
                            target.Patch(pair.Value);
                        }
                        break;
                }
            }

            return node;
        }

        public Node Normalized()
        {
            if (IsDictionary) {
                var node = new Node(_type);
                foreach (var kv in AsDictionary.OrderBy(kv => kv.Key)) {
                    node.AsDictionary.Add(kv.Key, kv.Value.Normalized());
                }
                return node;
            } else if (IsList) {
                var node = new Node(_type);
                foreach (var child in AsList.OrderBy(kv => kv.Key)) {
                    node.AsList.Add(child.Normalized());
                }
                return node;
            } else {
                return new Node(Type.String, AsString);
            }
        }
    }
}
