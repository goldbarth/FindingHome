using UnityEngine;

namespace Collectibles
{
    public class EdibleCounterTrigger : MonoBehaviour
    {
        public delegate void EdibleCollect();
        public static event EdibleCollect OnEdibleCollectEvent;
    
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                OnEdibleCollectEvent?.Invoke();
            }
        }
    }
}

