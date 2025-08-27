namespace JsonPath
{
    public class MemoryDataProvider : IDataProvider
    {
        public readonly Dictionary<string, string> Data = [];

        public bool HasData(string key)
        {
            return Data.ContainsKey(key);
        }

        public void SetData(string id, string value)
        {
            Data[id] = value;
        }

        public string? GetData(string id)
        {
            return Data.GetValueOrDefault(id);
        }
    }
}
