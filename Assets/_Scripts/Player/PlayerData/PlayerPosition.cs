using DataPersistence;
using Environment;
using ObstacleHandler;
using UnityEngine;

namespace Player.PlayerData
{
    public class PlayerPosition : MonoBehaviour, IDataPersistence
    {
        [SerializeField] private AudioSource audioSource;
        
        private void Start()
        {
            if (GameManager.Instance.IsNewGame)
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
            audioSource.Play();
            transform.position = closestRespawnPoint.position;
            DataPersistenceManager.Instance.LoadGame();
        }

        public void LoadData(GameData data)
        {
            transform.position = data.playerPosition;
        }

        public void SaveData(GameData data)
        {
            // prevents saving the player position when not changing the room (e.g. when falling into a pit)
            if (!GameManager.Instance.OnRoomReset)
                data.playerPosition = transform.position;
        }
    }
}