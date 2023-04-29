using System.Collections.Generic;

namespace BehaviorTree.Blackboard
{
    public class BtBlackboard : IBlackboard
    {
        private readonly Dictionary<string, object> _data = new();

        /// <summary>
        /// Sets the blackboard for the node.
        /// </summary>
        /// <param name="key">string</param>
        /// <param name="value">object</param>
        public void SetData<T>(string key, T value)
        {
            _data[key] = value;
        }

        /// <summary>
        /// Gets the data from the blackboard. If the data is not found, it will search in the parent node.
        /// </summary>
        /// <param name="node">Node</param>
        /// <param name="key">string</param>
        /// <param name="defaultValue">default</param>
        /// <returns>Returns the object(value) of the given key.</returns>
        public T GetData<T>(string key, T defaultValue = default)
        {
            if (_data.TryGetValue(key, out var value) && value is T result)
                return result;
            
            return defaultValue;
        }

        /// <summary>
        /// Removes the data from the blackboard. If the data is not found, it will search in the parent node.
        /// </summary>
        /// <param name="node">Node</param>
        /// <param name="key">string</param>
        /// <returns>Returns true if the data is successful removed</returns>
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
        
        //TODO: testing purpose. try implementing a method to store target id.
        private readonly Dictionary<(string, string), object> _objects = new();
        private string _objectID;

        public void SetObject(string key, object value)
        {
            _objects[(key, _objectID)] = value;
        }
        
        public Dictionary<(string, string), object> GetObject()
        {
            return _objects;
        }
    }
}