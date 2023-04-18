using System.Collections.Generic;

namespace AddIns
{
    public class TargetDictionary
    {
        private readonly Dictionary<string, TargetData> _dictionary;
        
        public TargetDictionary()
        {
            _dictionary = new Dictionary<string, TargetData>();   
        }
        
        public void Add(string key, string id, object value)
        {
            TargetData data = new(id, value);
            _dictionary.Add(key, data);
        }
        
        public TargetData Get(string key)
        {
            return _dictionary[key];
        }
        
        public void Set(string key, string id, object value)
        {
            TargetData data = new(id, value);
            _dictionary[key] = data;
        }
        
        public void Update(string key, string id, object value)
        {
            TargetData data = new(id, value);
            _dictionary[key] = data;
        }
        
        public void UpdateValue(string key, object value)
        {
            var data = _dictionary[key];
            data.Value = value;
            _dictionary[key] = data;
        }
        
        public bool ContainsKey(string key)
        {
            return _dictionary.ContainsKey(key);
        }
        
        public void Remove(string key)
        {
            _dictionary.Remove(key);
        }
    }
}