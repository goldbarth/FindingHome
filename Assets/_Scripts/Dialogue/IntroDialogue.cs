using UnityEngine;
using AddIns;

namespace Dialogue
{
    public class IntroDialogue : Singleton<IntroDialogue>
    {
        [SerializeField] private GameObject _popup;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private TextAsset _inkJson;
        
        private Controls _controls;
        private bool _inRange;

        protected override void Awake()
        {
            _inRange = false;
            _popup.SetActive(false);
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
            _popup.SetActive(_inRange);
            if (_inRange && !DialogueManager.Instance.OnDialogueActive())
                if (_controls.Gameplay.Interact.triggered)
                    DialogueManager.Instance.EnterDialogueMode(_inkJson);
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if(col.CompareTag("Player") && !col.isTrigger)
                _inRange = true;
            
            var hitPos = col.ClosestPoint(transform.position);
            _spriteRenderer.flipX = hitPos.x < transform.position.x;
        }
        
        private void OnTriggerExit2D(Collider2D col)
        {
            if(col.CompareTag("Player") && !col.isTrigger)
                _inRange = false;
            
            var hitPos = col.ClosestPoint(transform.position);
            _spriteRenderer.flipX = hitPos.x < transform.position.x;
        }
    }
}