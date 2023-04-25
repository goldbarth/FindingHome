using UnityEngine;

namespace Environment
{
    public class RespawnPoint : MonoBehaviour
    {
        [SerializeField] private Transform _spawnPoint;

        private void Awake()
        {
            _spawnPoint = GetComponent<Transform>();
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_spawnPoint.position, 0.2f);
        }
#endif
    }
}