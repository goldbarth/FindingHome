using System.Collections;
using DataPersistence;
using UnityEngine;

namespace RoomDesign
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private GameObject virtualCamera;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !other.isTrigger)
            {
                virtualCamera.SetActive(true);
                StartCoroutine(LateSave());
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !other.isTrigger)
            {
                virtualCamera.SetActive(false);
            }
        }
        
        private static IEnumerator LateSave()
        {
            yield return new WaitForSeconds(1f);
            DataPersistenceManager.Instance.SaveGame();
        }
    }
}
