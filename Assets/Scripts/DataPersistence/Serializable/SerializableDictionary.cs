using System;
using System.Collections.Generic;
using UnityEngine;

namespace DataPersistence.Serializable
{
    // This class is used to store the data of the dictionary in a serializable format that can be saved by .json.
    // Note: "JSON .NET For Unity" in asset store if it gets more complex.
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> keys = new ();
    
        [SerializeField]
        private List<TValue> values = new ();
    
        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
    
            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }
    
        public void OnAfterDeserialize()
        {
            Clear();
    
            if (keys.Count != values.Count)
                Debug.Log($"there are {keys.Count} keys and {values.Count} values after deserialization. " +
                          $"Make sure that both key and value types are serializable.");
    
            for (int i = 0; i < keys.Count; i++)
                Add(keys[i], values[i]);
        }
    }
}
