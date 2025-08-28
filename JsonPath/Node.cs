using System.Collections;
using System.Globalization;

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

        public Dictionary Add(string key, string value) { base.Add(key, Node.From(value)); return this; }
        public Dictionary Add(string key, int value) { base.Add(key, Node.From(value)); return this; }
        public Dictionary Add(string key, long value) { base.Add(key, Node.From(value)); return this; }
        public Dictionary Add(string key, float value) { base.Add(key, Node.From(value)); return this; }
        public Dictionary Add(string key, double value) { base.Add(key, Node.From(value)); return this; }
        public Dictionary Add(string key, bool value) { base.Add(key, Node.From(value)); return this; }
        public Dictionary Add(string key, DateTime value) { base.Add(key, Node.From(value)); return this; }
        public new void Add(string key, Node node) { base.Add(key, node); }
        public new Node this[string key] { get { return Get(key); } set { base[key] = value; } }
        public Node ToNode() { return Node.From(this); }
        public static implicit operator Node(Dictionary d) { return new Node(Node.Type.Dictionary) { Value = d }; }
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

        public void Add(string value) { base.Add(Node.From(value)); }
        public void Add(int value) { base.Add(Node.From(value)); }
        public void Add(long value) { base.Add(Node.From(value)); }
        public void Add(float value) { base.Add(Node.From(value)); }
        public void Add(double value) { base.Add(Node.From(value)); }
        public void Add(bool value) { base.Add(Node.From(value)); }
        public void Add(DateTime value) { base.Add(Node.From(value)); }
        public new void Add(Node node) { base.Add(node); }
        //public Node ToNode() { return Node.From(this); }
        public static implicit operator Node(List l) { return new Node(Node.Type.List) { Value = l }; }
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
        public long Int => AsInt;
        public bool Bool => AsBool;
        public string String => AsString;
        public double Float => AsFloat;
        public DateTime Date => AsDate;

        // For Xml
        public string Name => this[Xml.Name].AsString;
        public string Text => this[Xml.Text].AsString;
        public Dictionary Attributes => this[Xml.Attributes];
        public List Children => this[Xml.Children];

        public static implicit operator int(Node node) { return (int)node.AsInt; }
        public static implicit operator long(Node node) { return node.AsInt; }
        public static implicit operator bool(Node node) { return node.AsBool; }
        public static implicit operator string(Node node) { return node.AsString; }
        public static implicit operator double(Node node) { return node.AsFloat; }
        public static implicit operator DateTime(Node node) { return node.AsDate; }
        public static implicit operator List(Node node) { return node.AsList; }
        public static implicit operator Dictionary(Node node) { return node.AsDictionary; }

        public static implicit operator Node(string value) { return Node.From(value); }
        public static implicit operator Node(int value) { return Node.From(value); }
        public static implicit operator Node(long value) { return Node.From(value); }
        public static implicit operator Node(bool value) { return Node.From(value); }
        public static implicit operator Node(float value) { return Node.From(value); }
        public static implicit operator Node(double value) { return Node.From(value); }
        public static implicit operator Node(DateTime value) { return Node.From(value); }

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

        public Node(Type type)
        {
            _type = type;

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
                default:
                    Value = 0;
                    break;
            }
        }

        public static Node FromJson(string json, DeserializerOptions? options = null)
        {
            return Deserializer.FromJson(json, options);
        }

        public static Node FromXml(string xml, XmlDeserializerOptions? options = null)
        {
            return new XmlDeserializer(options).Parse(xml);
        }

        public static Node FromYaml(string yaml, YamlDeserializer.Options? options = null)
        {
            return YamlDeserializer.Deserialize(yaml, options);
        }

        public static Node FromKeyValueLf(string data)
        {
            return new KeyValueLfDeserializer().Parse(data);
        }

        public static Node From(Node n) { return new Node(n._type) { Value = n.Value }; }
        public static Node From(string value) { return new Node(Type.String) { Value = value }; }
        public static Node From(long value) { return new Node(Type.Int) { Value = value }; }
        public static Node From(int value) { return new Node(Type.Int) { Value = Convert.ToInt64(value) }; }
        public static Node From(double value) { return new Node(Type.Float) { Value = value }; }
        public static Node From(float value) { return new Node(Type.Float) { Value = Convert.ToDouble(value) }; }
        public static Node From(bool value) { return new Node(Type.Bool) { Value = value }; }
        public static Node From(DateTime value) { return new Node(Type.Date) { Value = value }; }

        public delegate Node ValueConverter<T>(T value);
        public static Node From<T>(Dictionary<string, T> dict, ValueConverter<T> valueConverter)
        {
            var node = new Node(Type.Dictionary);
            foreach (var pair in dict) {
                node.AsDictionary.Add(pair.Key, valueConverter(pair.Value));
            }
            return node;
        }

        public static Node From(Dictionary<string, string> dict) { return From(dict, s => Node.From(s)); }
        public static Node From(Dictionary<string, long> dict) { return From(dict, s => Node.From(s)); }
        public static Node From(Dictionary<string, int> dict) { return From(dict, s => Node.From(s)); }
        public static Node From(Dictionary<string, double> dict) { return From(dict, s => Node.From(s)); }
        public static Node From(Dictionary<string, float> dict) { return From(dict, s => Node.From(s)); }
        public static Node From(Dictionary<string, bool> dict) { return From(dict, s => Node.From(s)); }
        public static Node From(Dictionary<string, DateTime> dict) { return From(dict, s => Node.From(s)); }

        public static Node From(Dictionary<string, Node> dict)
        {
            var node = new Node(Type.Dictionary);
            foreach (var pair in dict) {
                node.AsDictionary.Add(pair.Key, pair.Value);
            }
            return node;
        }

        public static Node From(IDictionary<string, object> dict)
        {
            var node = new Node(Type.Dictionary);
            foreach (var pair in dict) {
                Node child;
                object value = pair.Value;

                if (value is string s) {
                    child = Node.From(s);
                } else if (value is long l) {
                    child = Node.From(l);
                } else if (value is int i) {
                    child = Node.From(i);
                } else if (value is double d) {
                    child = Node.From(d);
                } else if (value is float f) {
                    child = Node.From(f);
                } else if (value is bool b) {
                    child = Node.From(b);
                } else if (value is DateTime dt) {
                    child = Node.From(dt);
                    //} else if (value is IEnumerable<object> valueIEnumerable) {
                    //    child = Node.From(valueIEnumerable);
                } else if (value is IDictionary<string, object> valueIDictionary) {
                    child = Node.From(valueIDictionary);
                } else if (value == null) {
                    child = new Node(Type.Empty);
                } else {
                    child = Node.From(value.ToString() ?? "");

                    // if ToJson exists
                    //   child = Node.From(value.ToJson());
                    // else 
                    //   child = Node.Reflected(value) // with cycle detection
                }

                if (child != null) {
                    node.AsDictionary.Add(pair.Key, child);
                }
            }
            return node;
        }

        public static Node From(IEnumerable<string> list)
        {
            var node = new Node(Type.List);
            foreach (var item in list) {
                node.AsList.Add(Node.From(item));
            }
            return node;
        }

        public static Node From(IEnumerable<long> list)
        {
            var node = new Node(Type.List);
            foreach (var item in list) {
                node.AsList.Add(Node.From(item));
            }
            return node;
        }

        public static Node From(IEnumerable<Node> list)
        {
            var node = new Node(Type.List);
            foreach (var item in list) {
                node.AsList.Add(item);
            }
            return node;
        }

        public string ToJson(SerializerOptions options)
        {
            return Serializer.ToJson(this, options);
        }

        public string ToJson(bool spaced = false, bool indented = false)
        {
            return Serializer.ToJson(this, spaced, indented);
        }

        public string ToJsonFormatted()
        {
            return ToJson(true, true);
        }

        public string ToJavascript(bool spaced = true, bool indented = true)
        {
            var options = new SerializerOptions(spaced: spaced, indented: indented) { EncapsulateKeys = "", EncapsulateBadKeys = "'", EncapsulateStrings = "'", };
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
                foreach (var child in AsList.OrderBy(n => n.AsString)) {
                    node.AsList.Add(child.Normalized());
                }
                return node;
            } else {
                return Node.From(AsString);
            }
        }

    }
}
