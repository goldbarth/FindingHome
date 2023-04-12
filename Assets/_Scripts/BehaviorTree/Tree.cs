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
            // ReSharper disable once Unity.NoNullPropagation
            _root?.Evaluate();
        }

        protected abstract Node CreateTree();
    }
}