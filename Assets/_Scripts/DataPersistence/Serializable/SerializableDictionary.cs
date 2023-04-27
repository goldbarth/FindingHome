using System;
using System.Collections.Generic;
using UnityEngine;

namespace DataPersistence.Serializable
{
    // This class is used to store the data of the dictionary in a serializable format that can be saved by .json.
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<TKey> _keys = new();
        [SerializeField] private List<TValue> _values = new();
    
        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();
    
            foreach (var pair in this)
            {
                _keys.Add(pair.Key);
                _values.Add(pair.Value);
            }
        }
    
        public void OnAfterDeserialize()
        {
            Clear();
    
            if (_keys.Count != _values.Count)
                Debug.Log($"there are {_keys.Count} keys and {_values.Count} values after deserialization. " +
                          $"Make sure that both key and value types are serializable.");
    
            for (int i = 0; i < _keys.Count; i++)
                Add(_keys[i], _values[i]);
        }
    }
}
