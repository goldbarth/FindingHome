﻿using System.Collections.Generic;

namespace BehaviorTree.Blackboard
{
    public class Blackboard : IBlackboard
    {
        private readonly Dictionary<string, BlackboardItem> _data = new();

        /// <summary>
        /// Stores the data in the blackboard.
        /// </summary>
        /// <param name="key">string</param>
        /// <param name="id">string</param>
        /// <param name="value">T</param>
        public void SetData<T>(string key, string id, T value)
        {
            BlackboardItem item = new(id, value);
            _data[key] = item;
        }

        /// <summary>
        /// Get the data from the blackboard by searching with a key.
        /// </summary>
        /// <param name="key">string</param>
        /// <param name="defaultValue">T</param>
        /// <returns>Returns the object(value) of the given key.</returns>
        public T GetData<T>(string key, T defaultValue = default)
        {
            if (_data.TryGetValue(key, out var data) && data.Value is T result)
                return result;
            
            return defaultValue;
        }
        
        /// <summary>
        /// Get the data from the blackboard by searching with a key and id.
        /// </summary>
        /// <param name="data">Dictionary(string, BlackboardItem)</param>
        /// <param name="key">string</param>
        /// <param name="id">string</param>
        /// <param name="defaultValue">T</param>
        /// <typeparam name="T">generic</typeparam>
        /// <returns>Returns the object(value) of the given key and id.</returns>
        public T GetData<T>(Dictionary<string, BlackboardItem> data, string key, string id, T defaultValue = default)
        {
            if (data.TryGetValue(key, out var item) && item.ID == id && item.Value is T result)
                return result;

            return defaultValue;
        }
        
        /// <summary>
        /// Get the id from the blackboard by searching with a key.
        /// </summary>
        /// <param name="key">string</param>
        /// <returns>Returns the id if the searched key was found.</returns>
        public string GetId(string key)
        {
            if (_data.TryGetValue(key, out var data))
                return data.ID;

            return null;
        }

        /// <summary>
        /// Removes the data from the blackboard.
        /// </summary>
        /// <param name="key">string</param>
        public void ClearData(string key)
        {
            _data.Remove(key);
        }
        
        /// <summary>
        /// Checks if the blackboard contains the given key.
        /// </summary>
        /// <param name="key">string</param>
        /// <returns>Returns true if the searched key was found.</returns>
        public bool ContainsKey(string key)
        {
            return _data.ContainsKey(key);
        }
        
        /// <summary>
        /// Checks if the blackboard contains the given id.
        /// </summary>
        /// <param name="id">string</param>
        /// <returns>Returns true if the searched id was found.</returns>
        public bool ContainsId(string id)
        {
            foreach (var item in _data.Values)
                if (item.ID == id) return true;

            return false;
        }
        
        /// <summary>
        /// Checks if the blackboard contains the given key id in the context of a key.
        /// </summary>
        /// <param name="key">string</param>
        /// <param name="id">string</param>
        /// <returns>Returns true if the searched id was found.<br/> Returns the searched id as out parameter.</returns>
        public bool TryGetId(string key, out string id)
        {
            if (_data.TryGetValue(key, out var data))
            {
                id = data.ID; 
                return true;
            }

            id = null;
            return false;
        }
    }
}