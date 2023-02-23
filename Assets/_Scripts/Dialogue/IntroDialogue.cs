using UnityEngine;

namespace Dialogue
{
    public class IntroDialogue : MonoBehaviour
    {
        [SerializeField] private GameObject popup;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private TextAsset inkJson;
        
        private Controls _controls;
        private bool _inRange;

        private void Awake()
        {
            _inRange = false;
            popup.SetActive(false);
            _controls = new Controls();
        }

        private void OnEnable()
        {
            _controls.Enable();
        }
        
        private void OnDisable()
        {
            _controls.Disable();
        }

        private void Update()
        {
            Debug.Log(DialogueManager.Instance.OnDialogueIsActive);
            popup.SetActive(_inRange);
            if (_inRange && !DialogueManager.Instance.OnDialogueIsActive)
                if (_controls.Gameplay.Interact.triggered)
                    DialogueManager.Instance.EnterDialogueMode(inkJson);
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if(col.CompareTag("Player") && !col.isTrigger)
                _inRange = true;
            
            var hitPos = col.ClosestPoint(transform.position);
            spriteRenderer.flipX = hitPos.x < transform.position.x;
        }
        
        private void OnTriggerExit2D(Collider2D col)
        {
            if(col.CompareTag("Player") && !col.isTrigger)
                _inRange = false;
            
            var hitPos = col.ClosestPoint(transform.position);
            spriteRenderer.flipX = hitPos.x < transform.position.x;
        }
    }
}