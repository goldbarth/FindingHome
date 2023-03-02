using DataPersistence;
using Environment;
using ObstacleHandler;
using UnityEngine;

namespace Player.PlayerData
{
    public class PlayerPosition : MonoBehaviour, IDataPersistence
    {
        private void Start()
        {
            if (GameManager.Instance.IsNewGame || DataPersistenceManager.Instance.DisableDataPersistence)
                transform.position = FindObjectOfType<StartSpawnPoint>().SpawnPoint.position;
        }

        private void OnEnable()
        {
            RespawnCollision.OnRespawnEvent += RespawnPosition;
        }
        
        private void OnDisable()
        {
            RespawnCollision.OnRespawnEvent -= RespawnPosition;
        }

        private void RespawnPosition(Transform closestRespawnPoint)
        {
            transform.position = closestRespawnPoint.position;
            DataPersistenceManager.Instance.LoadGame();
        }

        public void LoadData(GameData data)
        { }

        public void SaveData(GameData data)
        {
            // prevents saving the player position when not changing the room (e.g. when falling into a pit)
            if (!GameManager.Instance.OnRoomReset)
            {
                data.playerPosition = transform.position;
            }
        }
    }
}
