using System.Collections;
using AnimationHandler;
using SceneHandler;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Extensions
{
    public class ExitSequence : MonoBehaviour
    {
        [SerializeField] private GameObject exitCanvas;
        [SerializeField] private GameObject exitAnimation;
        [SerializeField] private GameObject closingWords;
        [SerializeField] private GameObject menuButton;
        [SerializeField] private AudioSource audioSource;

        private void OnEnable()
        {
            LevelExitAnimation.OnLevelExitEvent += StartExitSequence;
        }

        private void OnDisable()
        {
            LevelExitAnimation.OnLevelExitEvent -= StartExitSequence;
        }
        
        private void StartExitSequence()
        {
            exitCanvas.SetActive(true);
            audioSource.enabled = true;
            StartCoroutine(DisableCharAnimation());
           
        }

        private IEnumerator DisableCharAnimation()
        {
            yield return new WaitForSeconds(4.5f);
            audioSource.pitch = 1f;
            yield return new WaitForSeconds(1.5f);
            audioSource.Stop();
            exitAnimation.SetActive(false);
            yield return new WaitForSeconds(5f);
            closingWords.SetActive(true);
            menuButton.gameObject.SetActive(true);
        }
    }
}