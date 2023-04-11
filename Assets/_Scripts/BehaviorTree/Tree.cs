using UnityEngine;

namespace BehaviorTree
{
    public abstract class Tree : MonoBehaviour
    {
        private Node _root = null;

        protected void Start()
        {
            _root = CreateTree();
        }

        private void Update()
        {
            if(_root != null) _root.Tick();
        }

        protected abstract Node CreateTree();
    }
}