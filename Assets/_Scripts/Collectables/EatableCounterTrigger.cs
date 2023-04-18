using UnityEngine;

namespace Collectables
{
    public class EatableCounterTrigger : MonoBehaviour
    {
        public delegate void EatableCountChanged();
        public static event EatableCountChanged OnEatableCountChangedEvent;
    
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                OnEatableCountChangedEvent?.Invoke();
            }
        }
    }
}

