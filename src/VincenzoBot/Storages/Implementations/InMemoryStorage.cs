using System;
using System.Collections.Generic;

namespace VincenzoBot.Storages
{
    public class InMemoryStorage : IDataStorage
    {
        private Dictionary<string, object> _dictionary = new Dictionary<string, object>();
        public T RestoreObject<T>(string key)
        {
            if (_dictionary.ContainsKey(key))
                throw new ArgumentException($"Provided key '{key}' wasn't found in dictionary.");
            return (T)_dictionary[key];
        }

        public void StoreObject(object obj, string key)
        {
            if (_dictionary.ContainsKey(key))
                throw new ArgumentException($"The key '{key}' already exists in the dictionary.");
            _dictionary.Add(key,obj);
        }
    }
}
