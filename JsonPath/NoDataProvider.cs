#nullable disable

namespace JsonPath
{
    public class NoDataProvider : IDataProvider
    {
        public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();

        public bool HasData(string key)
        {
            return false;
        }

        public void SetData(string id, string value)
        {
        }

        public string GetData(string id)
        {
            return null;
        }
    }
}
