namespace BehaviorTree.Blackboard
{
    public class BlackboardItem
    {
        public string ID { get; set; }
        public object Value { get; set; }
        
        public BlackboardItem(string id, object value)
        {
            ID = id;
            Value = value;
        }
    }
}