namespace AddIns
{
    public class TargetData
    {
        public string ID { get; set; }
        public object Value { get; set; }
        
        public TargetData(string id, object value)
        {
            ID = id;
            Value = value;
        }
    }
}