using System.Collections;
using Environment;
using UnityEngine;

namespace AnimationHandler
{
    public class SaveAnimation : MonoBehaviour
    {
        [Header("SAVE CANVAS")]
        [SerializeField] private GameObject saveCanvas;
        [SerializeField] private Animator saveCanvasAnimator;
        
        private readonly float _saveAnimationTime = 1.7f;

        private void OnDisable()
        {
            Room.OnRoomEnterEvent -= Play;
        }
        
        private void OnEnable()
        {
            Room.OnRoomEnterEvent += Play;
        }

        private void Play()
        {
            StartCoroutine(PlaySaveAnimation());
        }
        
        private IEnumerator PlaySaveAnimation()
        {
            saveCanvas.SetActive(true);
            saveCanvasAnimator.Play("save_text");
            yield return new WaitForSeconds(_saveAnimationTime);
            saveCanvas.SetActive(false);
        }
    }
}