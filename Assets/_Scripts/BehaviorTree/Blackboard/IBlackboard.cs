using System.Collections.Generic;

namespace BehaviorTree.Blackboard
{
    public interface IBlackboard
    {
        public string GetId(string key);
        T GetData<T>(Dictionary<string, Item> data, string key, string id, T defaultValue = default);
        T GetData<T>(string key, T defaultValue = default);
        void SetData<T>(string key, string id, T value);
        void ClearData(string key);
        bool ContainsKey(string key);
        bool ContainsId(string id);
        bool TryGetId(string key, out string id);
    }
}