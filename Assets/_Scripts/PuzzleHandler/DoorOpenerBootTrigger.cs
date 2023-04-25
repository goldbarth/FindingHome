using UnityEngine;

namespace PuzzleHandler
{
    public class DoorOpenerBootTrigger : MonoBehaviour
    {
        public delegate void OnBootCollision();
        public static event OnBootCollision OnBootCollisionEvent;

        private bool _booted;
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player") && !col.isTrigger && !_booted)
            {
                OnBootCollisionEvent?.Invoke();
                _booted = true;
            }
        }
    }
}