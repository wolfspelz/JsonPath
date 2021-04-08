using System.Collections.Generic;
using System.Threading.Tasks;

namespace JsonPath
{
    public class NoDataProvider : IDataProvider
    {
        public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();

        public async Task<bool> HasData(string key)
        {
            await Task.CompletedTask;
            return false;
        }

        public async Task SetData(string id, string value)
        {
            await Task.CompletedTask;
        }

        public async Task<string> GetData(string id)
        {
            await Task.CompletedTask;
            return null;
        }
    }
}
