#nullable disable

namespace JsonPath
{
    public class MemoryDataProvider : IDataProvider
    {
        public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();

        public bool HasData(string key)
        {
            return Data.ContainsKey(key);
        }

        public void SetData(string id, string value)
        {
            Data[id] = value;
        }

        public string GetData(string id)
        {
            if (Data.TryGetValue(id, out var value)) {
                return value;
            }
            return null;
        }
    }
}
