using System.Collections;
using DataPersistence;
using Environment;
using ObstacleHandler;
using UnityEngine;

namespace Player
{
    public class RespawnHandler : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private float _respawnTime = 0.9f;
        
        private Rigidbody2D _rigidBody;
        private Animator _animator;

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
            _audioSource.Play();
            StartCoroutine(RespawnPositionCoroutine(closestRespawnPoint));
        }

        private IEnumerator RespawnPositionCoroutine(Transform closestRespawnPoint)
        {
            DataPersistenceManager.Instance.LoadGame();
            transform.position = closestRespawnPoint.position;
            GameManager.Instance.IsRespawning = true;
            _animator.Play("player_appear_teleport");
            _rigidBody.constraints = RigidbodyConstraints2D.FreezePosition;
            yield return new WaitForSeconds(_respawnTime);
            _rigidBody.constraints = RigidbodyConstraints2D.None;
            _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
            GameManager.Instance.IsRespawning = false;
        }
    }
}