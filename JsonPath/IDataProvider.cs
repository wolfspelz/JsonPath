using System.Threading.Tasks;

namespace JsonPath
{
    public interface IDataProvider
    {
        Task<bool> HasData(string key);
        Task<string> GetData(string key);
        Task SetData(string key, string value);
    }
}