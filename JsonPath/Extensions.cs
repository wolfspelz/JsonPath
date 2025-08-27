namespace JsonPath
{
    public static class Extensions
    {
        public static JsonPath.Dictionary ToDictionary(this IEnumerable<KeyValuePair<string, Node>> self)
        {
            var dict = new Dictionary();
            foreach (var pair in self) {
                dict.Add(pair.Key, pair.Value);
            }
            return dict;
        }

        public static JsonPath.Node ToJsonNode(this string self)
        {
            return JsonPath.Node.FromJson(self);
        }

        public static JsonPath.Node ToJsonNode(this Dictionary<string, string> self)
        {
            return JsonPath.Node.From(self);
        }

        public static JsonPath.Node ToJsonNode(this Dictionary<string, object?> self)
        {
            return JsonPath.Node.From(self);
        }
    }
}
