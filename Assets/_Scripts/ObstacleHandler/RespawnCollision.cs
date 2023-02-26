using DataPersistence;
using Player.PlayerData;
using UnityEngine;

namespace ObstacleHandler
{
    public class RespawnCollision : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player") && !col.isTrigger)
            {
                GameManager.Instance.OnRoomReset = true;
                DataPersistenceManager.Instance.LoadGame();
                DeathCount.Instance.OnPlayerDeath();
            }
        }
    }
}
