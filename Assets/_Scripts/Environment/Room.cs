using System.Collections;
using DataPersistence;
using UnityEngine;

namespace Environment
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private GameObject virtualCamera;
        
        public delegate void OnRoomEnter();
        public static event OnRoomEnter OnRoomEnterEvent;
        
        private static readonly float TimeTillSetFlag = 1f;

        private void Awake()
        {
            StartCoroutine(WaitToSetFlagGameStarted());
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player") && !col.isTrigger)
            {
                virtualCamera.SetActive(true);
                StartCoroutine(LateSave());
                if (!GameManager.Instance.IsGameStarted)
                    OnRoomEnterEvent?.Invoke();
            }
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (col.CompareTag("Player") && !col.isTrigger)
            {
                virtualCamera.SetActive(false);
            }
        }

        private static IEnumerator WaitToSetFlagGameStarted()
        {
            yield return new WaitForSeconds(TimeTillSetFlag);
            GameManager.Instance.IsGameStarted = true;
        }
        
        private static IEnumerator LateSave()
        {
            yield return new WaitForSeconds(.5f);
            DataPersistenceManager.Instance.SaveGame();
        }
    }
}
