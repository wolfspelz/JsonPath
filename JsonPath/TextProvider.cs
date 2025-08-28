#nullable disable

namespace JsonPath
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Really?")]
    public class TextProvider : ITextProvider
    {
        public IDataProvider Cache { get; set; } = new MemoryDataProvider();
        public IDataName NameBuilder { get; set; } = new LodashDataName();

        private readonly IDataProvider _content;
        private readonly string _app;
        private readonly string _lang;
        private readonly string _context;

        public TextProvider(IDataProvider dataProvider, string appName, string langName, string contextName)
        {
            _content = dataProvider;
            _app = appName;
            _lang = langName;
            _context = contextName;
        }

        public string String(string key = null, string lang = null, string path = null, bool usePathAsDefault = true, StringGenerator data = null, Dictionary<string, StringGenerator> i18n = null)
        {
            var text = GetData(key, lang);
            string err = "";

            if (!string.IsNullOrEmpty(text)) {
                if (string.IsNullOrEmpty(path)) { return text; }
                try {
                    var node = JsonPath.Node.FromJson(text);
                    var child = ChildByPath(node, path);
                    if (child != null) {
                        return child.String;
                    }
                } catch (Exception ex) {
                    err = ex.Message;
                }
            }

            if (data != null) {
                return data();
            }

            if (i18n != null && _lang != null) {
                if (i18n.TryGetValue(_lang, out var i18nData)) {
                    return i18nData();
                }
            }

            if (usePathAsDefault) {
                if (!string.IsNullOrEmpty(path)) {
                    var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 0) { return parts[parts.Length - 1]; }
                }
            }

            return FormatError(key, lang, path, err);
        }

        public List<JsonPath.Node> List(string key = null, string lang = null, string path = null, ListGenerator data = null, Dictionary<string, ListGenerator> i18n = null)
        {
            var text = GetData(key, lang);
            string err = "";

            if (!string.IsNullOrEmpty(text)) {
                try {
                    var node = JsonPath.Node.FromJson(text);
                    if (string.IsNullOrEmpty(path)) { return node; }
                    var child = ChildByPath(node, path);
                    if (child != null) {
                        return child.List;
                    }
                } catch (Exception ex) {
                    err = ex.Message;
                }
            }

            if (data != null) {
                return data();
            }

            if (i18n != null) {
                if (i18n.TryGetValue(_lang, out var i18nData)) {
                    return i18nData();
                }
            }

            return new JsonPath.List() { FormatError(key, lang, path, err) };
        }

        public Dictionary<string, JsonPath.Node> Dictionary(string key = null, string lang = null, string path = null, DictionaryGenerator data = null, Dictionary<string, DictionaryGenerator> i18n = null)
        {
            var text = GetData(key, lang);
            string err = "";

            if (!string.IsNullOrEmpty(text)) {
                try {
                    var node = JsonPath.Node.FromJson(text);
                    if (string.IsNullOrEmpty(path)) { return node; }
                    var child = ChildByPath(node, path);
                    if (child != null) {
                        return child.Dictionary;
                    }
                } catch (Exception ex) {
                    err = ex.Message;
                }
            }

            if (data != null) {
                return data();
            }

            if (i18n != null) {
                if (i18n.TryGetValue(_lang, out var i18nData)) {
                    return i18nData();
                }
            }

            return new JsonPath.Dictionary() { ["error"] = FormatError(key, lang, path, err) };
        }

        public string Json(string key = null, string lang = null, string path = null)
        {
            var text = GetData(key, lang);
            string err = "";

            if (!string.IsNullOrEmpty(text)) {
                if (string.IsNullOrEmpty(path)) { return text; }
                try {
                    var node = JsonPath.Node.FromJson(text);
                    var child = ChildByPath(node, path);
                    if (child != null) {
                        return child.ToJson();
                    }
                } catch (Exception ex) {
                    err = ex.Message;
                }
            }

            return FormatError(key, lang, path, err);
        }

        public void Set(string key, string value)
        {
            var name = GetName(_lang, key);
            _content.SetData(name, value);
        }

        private string GetName(string lang, string key)
        {
            return NameBuilder.GetName(_app, lang, _context, key);
        }

        public string GetData(string key, string lang)
        {
            if (lang == null) { lang = _lang; }
            var name = GetName(lang, key);
            string text;

            if (Cache.HasData(name)) {
                text = Cache.GetData(name);
            } else {
                text = _content.GetData(name);
                if (text == null) {
                    var languageIndependentId = GetName("", key);
                    text = _content.GetData(languageIndependentId);
                }
                if (text != null) {
                    Cache.SetData(name, text);
                }
            }

            return text;
        }

        private string FormatError(string key, string lang, string path, string error)
        {
            return $"name={GetName(lang, key)} path={path} message={error}";
        }

        private Node ChildByPath(Node node, string path)
        {
            var parts = path.Split('/', 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0) {
                var segment = parts[0];
                if (node.AsDictionary.ContainsKey(segment)) {
                    var child = node[segment];
                    if (parts.Length == 2) {
                        var restOfPath = parts[1];
                        return ChildByPath(child, restOfPath);
                    } else {
                        return child;
                    }
                }
            }
            return null;
        }

    }
}
