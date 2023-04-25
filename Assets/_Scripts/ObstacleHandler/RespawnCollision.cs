using UnityEngine;

namespace ObstacleHandler
{
    public class RespawnCollision : MonoBehaviour
    { 
        [SerializeField] private Transform _closestRespawnPoint;
        public delegate void TrapCollision();
        public static event TrapCollision OnTrapCollisionEvent;
        public delegate void Respawn(Transform closestRespawnPoint);
        public static event Respawn OnRespawnEvent;
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player") && !col.isTrigger)
            {
                GameManager.Instance.OnRoomReset = true;
                OnTrapCollisionEvent?.Invoke();
                OnRespawnEvent?.Invoke(_closestRespawnPoint);
            }
        }
    }
}
