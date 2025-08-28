#nullable disable

namespace JsonPath
{
    public delegate string StringGenerator();
    public delegate List<JsonPath.Node> ListGenerator();
    public delegate Dictionary<string, JsonPath.Node> DictionaryGenerator();
    public class StringGeneratorI18n : Dictionary<string, StringGenerator> { };
    public class ListGeneratorI18n : Dictionary<string, ListGenerator> { };
    public class DictionaryGeneratorI18n : Dictionary<string, DictionaryGenerator> { };

    public interface ITextProvider
    {
        string String(string key = null, string lang = null, string path = null, bool usePathAsDefault = true, StringGenerator data = null, Dictionary<string, StringGenerator> i18n = null);
        List<JsonPath.Node> List(string key = null, string lang = null, string path = null, ListGenerator data = null, Dictionary<string, ListGenerator> i18n = null);
        Dictionary<string, JsonPath.Node> Dictionary(string key = null, string lang = null, string path = null, DictionaryGenerator data = null, Dictionary<string, DictionaryGenerator> i18n = null);
        string Json(string key = null, string lang = null, string path = null);
        void Set(string key, string value);
    }
}