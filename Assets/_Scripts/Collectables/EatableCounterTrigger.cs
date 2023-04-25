using UnityEngine;

namespace Collectables
{
    public class EatableCounterTrigger : MonoBehaviour
    {
        public delegate void EatableCollect();
        public static event EatableCollect OnEatableCollectEvent;
    
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                OnEatableCollectEvent?.Invoke();
            }
        }
    }
}

