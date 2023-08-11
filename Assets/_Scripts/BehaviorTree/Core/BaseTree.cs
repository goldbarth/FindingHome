using UnityEngine;

namespace BehaviorTree.Core
{
    // Sources: Christoph Graf @ KI Basics(Chapter Two).
    // I found almost the same logic also here: https://github.com/MinaPecheux/UnityTutorials-BehaviourTrees
    // and https://www.behaviortree.dev/docs/intro
    public abstract class BaseTree : MonoBehaviour
    {
        private BaseNode _root = null;

        protected virtual void Start()
        {
            _root = SetupTree();
        }

        private void Update()
        {
            _root?.Evaluate();
        }

        protected abstract BaseNode SetupTree();
    }
}