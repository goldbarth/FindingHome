using DataPersistence;
using Player.PlayerData;
using UnityEngine;

namespace ObstacleHandler
{
    public class RespawnCollision : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.Instance.OnRoomReset = true;
                DataPersistenceManager.Instance.SaveGame();
                DataPersistenceManager.Instance.LoadGame();
                DeathCount.Instance.OnPlayerDeath();
            }
        }
    }
}
