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
        [SerializeField] private float respawnTime = 0.7f;
        
        private Animator _animator;
        
        private void Start()
        {
            if (GameManager.Instance.IsNewGame)
                transform.position = FindObjectOfType<StartSpawnPoint>().SpawnPoint.position;
            
            _animator = GetComponentInChildren<Animator>();
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
            //yield return new WaitForSeconds(0.3f);
            DataPersistenceManager.Instance.LoadGame();
            transform.position = closestRespawnPoint.position;
            _animator.SetBool("IsAppearing", true);
            yield return new WaitForSeconds(respawnTime);
            _animator.SetBool("IsAppearing", false);
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