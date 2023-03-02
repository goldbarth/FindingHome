using UnityEngine;

namespace ObstacleHandler
{
    public class RespawnCollision : MonoBehaviour
    { 
        [SerializeField] private Transform closestRespawnPoint;
        public delegate void OnTrapCollision();
        public delegate void OnRespawn(Transform closestRespawnPoint);
        public static event OnTrapCollision OnTrapCollisionEvent;
        public static event OnRespawn OnRespawnEvent;
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player") && !col.isTrigger)
            {
                GameManager.Instance.OnRoomReset = true;
                OnTrapCollisionEvent?.Invoke();
                OnRespawnEvent?.Invoke(closestRespawnPoint);
            }
        }
    }
}
