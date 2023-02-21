using DataPersistence;
using UnityEngine;

namespace Player.PlayerData
{
    public class StartPosition : MonoBehaviour
    {
        private void Start()
        {
            if (GameManager.Instance.IsNewGame || DataPersistenceManager.Instance.DisableDataPersistence)
                transform.position = FindObjectOfType<StartSpawnPoint>().SpawnPoint.position;
        }
    }
}