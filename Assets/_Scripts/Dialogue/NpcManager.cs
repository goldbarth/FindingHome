using System.Collections;
using AddIns;
using Ink.Runtime;
using TMPro;
using UnityEngine;

namespace Dialogue
{
    public class NpcManager : Singleton<NpcManager>
    {
        private const float WaitTillCanMove = .2f;
        
        [SerializeField] private GameObject _dialoguePanel;
        [SerializeField] private TextMeshProUGUI _dialogueText;
        [SerializeField] private AudioSource _audioSource;
        
        private Controls _controls;
        private Story _currentStory;

        private bool _onDialogueIsActive = false;

        public bool IsInDialogue { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            
            _controls = new Controls();
            _onDialogueIsActive = false;
            _dialoguePanel.SetActive(false);
            IsInDialogue = false;
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
            if (_controls.UI.Submit.triggered && _onDialogueIsActive)
                ContinueDialogue();
        }

        public void EnterDialogueMode(TextAsset inkJson)
        {
            IsInDialogue = true;
            _audioSource.Play();
            _currentStory = new Story(inkJson.text);
            _onDialogueIsActive = true;
            _dialoguePanel.SetActive(true);
            
            ContinueDialogue();
        }
        
        private void ContinueDialogue()
        {
            if (_currentStory.canContinue)
                _dialogueText.text = _currentStory.Continue();
            else
                StartCoroutine(ExitDialogueMode());
        }
        
        public bool OnDialogueActive()
        {
            return _onDialogueIsActive;
        }

        private IEnumerator ExitDialogueMode()
        {
            _audioSource.Stop();
            yield return new WaitForSeconds(WaitTillCanMove);
            _onDialogueIsActive = false;
            _dialoguePanel.SetActive(false);
            _dialogueText.text = string.Empty;
            IsInDialogue = false;
        }
    }
}