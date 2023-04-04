using System.Collections;
using DataPersistence;
using Environment;
using ObstacleHandler;
using UnityEngine;

namespace Player.PlayerData
{
    public class PlayerPosition : MonoBehaviour, IDataPersistence
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private float respawnTime = 0.9f;
        
        private Animator _animator;
        private Rigidbody2D _rigidBody;

        private void Start()
        {
            if (GameManager.Instance.IsNewGame)
                transform.position = FindObjectOfType<StartSpawnPoint>().SpawnPoint.position;
            
            _animator = GetComponentInChildren<Animator>();
            _rigidBody = GetComponent<Rigidbody2D>();
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
            StartCoroutine(RespawnPositionCoroutine(closestRespawnPoint));
        }
        
        private IEnumerator RespawnPositionCoroutine(Transform closestRespawnPoint)
        {
            DataPersistenceManager.Instance.LoadGame();
            transform.position = closestRespawnPoint.position;
            GameManager.Instance.IsRespawning = true;
            _animator.Play("player_appear_teleport");
            _rigidBody.constraints = RigidbodyConstraints2D.FreezePosition;
            yield return new WaitForSeconds(respawnTime);
            _rigidBody.constraints = RigidbodyConstraints2D.None;
            _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
            GameManager.Instance.IsRespawning = false;
        }

        public void LoadData(GameData data)
        {
            transform.position = data.playerPosition;
        }

        public void SaveData(GameData data)
        {
            if (GameManager.Instance.IsGameStarted && !GameManager.Instance.IsGamePaused)
                data.playerPosition = transform.position;
        }
    }
}