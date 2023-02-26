using System;
using System.Collections;
using AnimationHandler;
using DataPersistence;
using UnityEngine;

namespace RoomDesign
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private GameObject virtualCamera;
        
        private SaveAnimation _saveAnimation;
        private static readonly float TimeTillSetFlag = 1f;

        private void Awake()
        {
            _saveAnimation = FindObjectOfType<SaveAnimation>();
            StartCoroutine(WaitToSetFlag());
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player") && !col.isTrigger)
            {
                Debug.Log("Entered room");
                virtualCamera.SetActive(true);
                StartCoroutine(LateSave());
                if (!GameManager.Instance.IsGameStarted)
                    StartCoroutine(_saveAnimation.PlaySaveAnimation());
            }
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (col.CompareTag("Player") && !col.isTrigger)
            {
                virtualCamera.SetActive(false);
            }
        }

        private static IEnumerator WaitToSetFlag()
        {
            yield return new WaitForSeconds(TimeTillSetFlag);
            GameManager.Instance.IsGameStarted = false;
        }
        
        private static IEnumerator LateSave()
        {
            yield return new WaitForSeconds(.5f);
            DataPersistenceManager.Instance.SaveGame();
        }
    }
}
