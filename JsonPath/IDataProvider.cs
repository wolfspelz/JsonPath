namespace JsonPath
{
    public interface IDataProvider
    {
        bool HasData(string key);
        string? GetData(string key);
        void SetData(string key, string value);
    }
}