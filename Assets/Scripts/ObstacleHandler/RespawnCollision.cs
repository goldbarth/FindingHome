using DataPersistence;
using UnityEngine;

namespace ObstacleHandler
{
    public class RespawnCollision : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                DataPersistenceManager.Instance.LoadGame();
            }
        }
    }
}
