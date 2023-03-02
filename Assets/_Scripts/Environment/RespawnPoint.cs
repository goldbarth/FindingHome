using UnityEngine;

namespace Environment
{
    public class RespawnPoint : MonoBehaviour
    {
        [SerializeField] private Transform spawnPoint;
        public Transform SpawnPoint => spawnPoint;

        private void Awake()
        {
            spawnPoint = GetComponent<Transform>();
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(spawnPoint.position, 0.2f);
        }
#endif
    }
}