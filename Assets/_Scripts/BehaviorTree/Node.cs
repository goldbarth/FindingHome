using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    // Sources: Christoph Graf´s KI Basics and https://www.behaviortree.dev/docs/category/learn-the-basic-concepts
    public enum ReturnStat
    {
        SUCCESS,
        FAILURE,
        RUNNING
    }

    public abstract class Node : MonoBehaviour
    {
        protected ReturnStat Result;
        public Node Parent;
        protected List<Node> Children = new();
        
        private Dictionary<string, object> _data = new();
        
        public Node()
        {
            Parent = null;
        }
        
        public Node(List<Node> children)
        {
            foreach(var child in children)
                AddChild(child);
        }
        
        public void AddChild(Node child)
        {
            Children.Add(child);
        }
        
        public virtual ReturnStat Tick() => ReturnStat.FAILURE;
        
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