using System.Collections.Generic;
using System.Threading.Tasks;

namespace JsonPath
{
    public class MemoryDataProvider : IDataProvider
    {
        public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();

        public async Task<bool> HasData(string key)
        {
            await Task.CompletedTask;
            return Data.ContainsKey(key);
        }

        public async Task SetData(string id, string value)
        {
            await Task.CompletedTask;
            Data[id] = value;
        }

        public async Task<string> GetData(string id)
        {
            await Task.CompletedTask;
            if (Data.TryGetValue(id, out var value)) {
                return value;
            }
            return null;
        }
    }
}
