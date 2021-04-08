using System.Text;

namespace JsonPath
{
    public class LodashDataName : IDataName
    {
        public string GetName(string app, string lang, string context, string key)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(app)) { sb.Append(app); }
            if (!string.IsNullOrEmpty(lang)) {
                if (sb.Length > 0) { sb.Append("_"); }
                sb.Append(lang.Replace("-", "_"));
            }
            if (!string.IsNullOrEmpty(context)) {
                if (sb.Length > 0) { sb.Append("_"); }
                sb.Append(context);
            }
            if (!string.IsNullOrEmpty(key)) {
                if (sb.Length > 0) { sb.Append("_"); }
                sb.Append(key);
            }
            return sb.ToString();
        }
    }
}
