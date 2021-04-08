using System.Threading.Tasks;
using System.IO;
using System;

namespace JsonPath
{
    public class ReadonlyFileDataProvider : IDataProvider
    {
        private readonly string _basePath;

        public ReadonlyFileDataProvider(string basePath = null)
        {
            _basePath = basePath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\i18n");
        }

        public string FilePath(string id)
        {
            return Path.Combine(_basePath, id) + ".json";
        }

        public async Task<bool> HasData(string id)
        {
            await Task.CompletedTask;
            return File.Exists(FilePath(id));
        }

        public async Task SetData(string id, string value)
        {
            await Task.CompletedTask;
            throw new Exception("Writing data to file system not supported");
        }

        public async Task<string> GetData(string id)
        {
            await Task.CompletedTask;
            var path = FilePath(id);
            if (!File.Exists(path)) {
                return null;
            }

            try {
                var text = File.ReadAllText(path);
                return text;
            } catch (Exception ex) {
                return ex.Message;
            }
        }
    }
}
