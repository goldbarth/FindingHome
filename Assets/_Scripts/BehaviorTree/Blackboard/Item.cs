namespace BehaviorTree.Blackboard
{
    public class Item
    {
        public string ID { get; set; }
        public object Value { get; set; }
        
        public Item(string id, object value)
        {
            ID = id;
            Value = value;
        }
    }
}