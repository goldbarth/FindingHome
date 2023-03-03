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
        //[SerializeField] private Animator exitAnimator;
        

        private readonly float _waitToPitch = 4.5f;
        private readonly float _stopAudio = 1.5f;
        private readonly float _displayEndScreen = 2f;
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
            yield return new WaitForSeconds(_waitToPitch);
            stepAudio.pitch = 1f;
            yield return new WaitForSeconds(_stopAudio);
            stepAudio.Stop();
            walkAnimation.SetActive(false);
            yield return new WaitForSeconds(_displayEndScreen);
            closingWords.SetActive(true);
            menuButton.gameObject.SetActive(true);
        }
    }
}