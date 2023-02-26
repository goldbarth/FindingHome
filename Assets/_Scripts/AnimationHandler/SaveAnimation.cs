using System.Collections;
using UnityEngine;

namespace AnimationHandler
{
    public class SaveAnimation : MonoBehaviour
    {
        [Header("SAVE CANVAS")]
        [SerializeField] private GameObject saveCanvas;
        [SerializeField] private Animator saveCanvasAnimator;
        
        public IEnumerator PlaySaveAnimation()
        {
            saveCanvas.SetActive(true);
            saveCanvasAnimator.Play("save_text");
            yield return new WaitForSeconds(2f);
            saveCanvas.SetActive(false);
        }
    }
}