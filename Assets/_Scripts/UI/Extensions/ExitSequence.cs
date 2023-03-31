using System.Collections;
using AnimationHandler;
using SceneHandler;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.Extensions
{
    public class ExitSequence : MonoBehaviour
    {
        [SerializeField] private GameObject exitCanvas;
        [SerializeField] private GameObject teleportAnimation;
        [SerializeField] private GameObject walkAnimation;
        [SerializeField] private GameObject closingWords;
        [SerializeField] private GameObject menuButton;
        [SerializeField] private AudioSource stepAudio;
        [SerializeField] private AudioSource teleportAudio;

        private const float WAIT_TO_PITCH = 4.5f;
        private const float STOP_AUDIO = 1.5f;
        private const float DISPLAY_END_SCREEN = 2f;

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
            StartCoroutine(CharExitSequence());
        }

        private IEnumerator CharExitSequence()
        {
            teleportAudio.Play();
            teleportAnimation.SetActive(true);
            yield return new WaitForSeconds(.8f);
            teleportAnimation.SetActive(false);
            walkAnimation.SetActive(true);
            teleportAudio.Stop();
            stepAudio.enabled = true;
            yield return new WaitForSeconds(WAIT_TO_PITCH);
            stepAudio.pitch = 1f;
            yield return new WaitForSeconds(STOP_AUDIO);
            stepAudio.Stop();
            walkAnimation.SetActive(false);
            yield return new WaitForSeconds(DISPLAY_END_SCREEN);
            closingWords.SetActive(true);
            menuButton.gameObject.SetActive(true);
        }
    }
}