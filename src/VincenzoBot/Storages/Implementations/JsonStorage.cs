using Moq;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

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
            File.WriteAllText(file, json);
        }

        public void UpdateObject(object obj, string oldPath, string newPath)
        {
            if (Exists($"{oldPath}.json"))
                File.Move($"{oldPath}.json", $"{newPath}.json");
            else
                throw new NullReferenceException("Path of old object doesnt exist.");
        }

        public static implicit operator JsonStorage(Mock<JsonStorage> v)
        {
            return v;
        }
    }
}
