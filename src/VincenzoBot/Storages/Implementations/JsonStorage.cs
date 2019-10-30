using Moq;
using Newtonsoft.Json;
using System;
using System.IO;

namespace VincenzoBot.Storages.Implementations
{
    public class JsonStorage : IDataStorage
    {
        public void DeleteObject(string key)
        {
            File.Delete($"{key}.json");    
        }

        public bool Exists(string filePath)
        {
            return File.Exists(filePath);
        }

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

        public static implicit operator JsonStorage(Mock<JsonStorage> v)
        {
            return v;
        }
    }
}
