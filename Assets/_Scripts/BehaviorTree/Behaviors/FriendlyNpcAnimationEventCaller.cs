using UnityEngine;

namespace BehaviorTree.Behaviors
{
    public class FriendlyNpcAnimationEventCaller : MonoBehaviour
    {
        private FriendlyNpcBehavior _friendlyNpcBehavior;

        private void Awake()
        {
            _friendlyNpcBehavior = transform.parent.GetComponentInChildren<FriendlyNpcBehavior>();
        }

        public void CallSpitterEvent(SpitterAnimationEvents toCall)
        {
            _friendlyNpcBehavior.CallAnimationEvent(toCall);
        }
    }
}