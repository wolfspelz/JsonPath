using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JsonPath;
using System.Text;

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

        public async Task<string> String(string key = null, string path = null, bool usePathAsDefault = true, StringGenerator data = null, Dictionary<string, StringGenerator> i18n = null)
        {
            var text = await GetData(key);
            string err = "";

            if (!string.IsNullOrEmpty(text)) {
                if (string.IsNullOrEmpty(path)) { return text; }
                try {
                    var node = new JsonPath.Node(text);
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

            if (i18n != null) {
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

            return FormatError(key, path, err);
        }

        public async Task<List<JsonPath.Node>> List(string key = null, string path = null, ListGenerator data = null, Dictionary<string, ListGenerator> i18n = null)
        {
            var text = await GetData(key);
            string err = "";

            if (!string.IsNullOrEmpty(text)) {
                try {
                    var node = new JsonPath.Node(text);
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

            return new JsonPath.List() { FormatError(key, path, err) };
        }

        public async Task<Dictionary<string, JsonPath.Node>> Dictionary(string key = null, string path = null, DictionaryGenerator data = null, Dictionary<string, DictionaryGenerator> i18n = null)
        {
            var text = await GetData(key);
            string err = "";

            if (!string.IsNullOrEmpty(text)) {
                try {
                    var node = new JsonPath.Node(text);
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

            return new JsonPath.Dictionary() { ["error"] = FormatError(key, path, err) };
        }

        public async Task<string> Json(string key = null, string path = null)
        {
            var text = await GetData(key);
            string err = "";

            if (!string.IsNullOrEmpty(text)) {
                if (string.IsNullOrEmpty(path)) { return text; }
                try {
                    var node = new JsonPath.Node(text);
                    var child = ChildByPath(node, path);
                    if (child != null) {
                        return child.ToJson();
                    }
                } catch (Exception ex) {
                    err = ex.Message;
                }
            }

            return FormatError(key, path, err);
        }

        public async Task Set(string key, string value)
        {
            var name = GetName(_lang, key);
            await _content.SetData(name, value);
        }

        private string GetName(string lang, string key)
        {
            return NameBuilder.GetName(_app, lang, _context, key);
        }

        public async Task<string> GetData(string key)
        {
            var name = GetName(_lang, key);
            string text;

            if (await Cache.HasData(name)) {
                text = await Cache.GetData(name);
            } else {
                text = await _content.GetData(name);
                if (text == null) {
                    var languageIndependentId = GetName("", key);
                    text = await _content.GetData(languageIndependentId);
                }
                if (text != null) {
                    await Cache.SetData(name, text);
                }
            }

            return text;
        }

        private string FormatError(string key, string path, string error)
        {
            return $"name={GetName(_lang, key)} path={path} message={error}";
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
