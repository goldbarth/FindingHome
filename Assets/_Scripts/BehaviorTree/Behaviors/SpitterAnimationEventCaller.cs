using UnityEngine;

namespace BehaviorTree.Behaviors
{
    public enum SpitterAnimationEvents
    {
        ChangeController,
    }
    
    public class SpitterAnimationEventCaller : MonoBehaviour
    {
        private SpitterBehavior _spitterBehavior;

        private void Awake()
        {
            _spitterBehavior = transform.parent.GetComponentInChildren<SpitterBehavior>();
        }

        public void CallSpitterEvent(SpitterAnimationEvents toCall)
        {
            _spitterBehavior.CallAnimationEvent(toCall);
        }
    }
}