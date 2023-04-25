using UnityEngine;

namespace Environment
{
    public class StartSpawnPoint : MonoBehaviour
    {
        [SerializeField] private Transform _spawnPoint;
        public Transform SpawnPoint => _spawnPoint;

        private void Awake()
        {
            _spawnPoint = GetComponent<Transform>();
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_spawnPoint.position, 0.2f);
        }
#endif
    }
}