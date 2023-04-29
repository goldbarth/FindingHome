namespace BehaviorTree.Blackboard
{
    public interface IBlackboard
    {
        T GetData<T>(string key, T defaultValue = default);
        void SetData<T>(string key, T value);
        void ClearData(string key);
        bool ContainsKey(string key);
    }
}