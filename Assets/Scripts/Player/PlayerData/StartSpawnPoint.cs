using UnityEngine;

namespace Player.PlayerData
{
    public class StartSpawnPoint : MonoBehaviour
    {
        [SerializeField] private Transform spawnPoint;
        public Transform SpawnPoint => spawnPoint;

        private void Awake()
        {
            spawnPoint = GetComponent<Transform>();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(spawnPoint.position, 0.2f);
        }

    }
}