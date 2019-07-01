using Newtonsoft.Json;
using System.IO;

namespace VincenzoBot.Storages.Implementations
{
    public class JsonStorage : IDataStorage
    {
        public T RestoreObject<T>(string key)
        {
            if (!File.Exists($"{key}.json"))
                return default;
            var json = File.ReadAllText($"{key}.json");
            return JsonConvert.DeserializeObject<T>(json);
        }

        public void StoreObject(object obj, string key)
        {
            var file = $"{key}.json";
            Directory.CreateDirectory(Path.GetDirectoryName(file));
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            File.WriteAllText(file,json);
        }
    }
}
