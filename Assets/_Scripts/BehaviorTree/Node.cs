using System.Collections.Generic;

namespace BehaviorTree
{
    // Sources: Christoph Graf´s KI Basics, https://github.com/MinaPecheux/UnityTutorials-BehaviourTrees
    // and https://www.behaviortree.dev/docs/intro
    public abstract class Node
    {
        public Node Parent;
        
        protected List<Node> Children = new();
        protected NodeState State;

        //TODO: testing purpose. try implementing a method to store target id.
        private Dictionary<(string, string), object> _objects = new();
        private Dictionary<string, object> _data = new();
        private string _targetID;
        
        public Node()
        {
            Parent = null;
        }
        
        public Node(List<Node> children)
        {
            foreach(var child in children)
                AddChild(child);
        }
        
        public void AddChild(Node node)
        {
            node.Parent = this;
            Children.Add(node);
        }
        
        public virtual NodeState Evaluate() => NodeState.Failure;

        #region ID_Tests
        
        public void SetObject(string key, object value)
        {
            _objects[(key, _targetID)] = value;
        }
        
        public Dictionary<(string, string), object> GetObject()
        {
            return _objects;
        }
        #endregion

        public void SetData(string key, object value)
        {
            _data[key] = value;
        }
        
        public object GetData(string key)
        {
            if(_data.TryGetValue(key, out var value))
                return value;
            
            var node = Parent;
            while (node != null)
            {
                value = node.GetData(key);
                if (value != null) return value;
                node = node.Parent;    
            }
            
            return null;
        }
        
        public bool ClearData(string key)
        {
            if (_data.ContainsKey(key))
            {
                return _data.Remove(key);
            }
            
            var node = Parent;
            while (node != null)
            {
                if (node.ClearData(key)) return true;
                node = node.Parent;    
            }
            
            return false;
        }
    }
}