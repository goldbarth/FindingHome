using System.Collections;
using AddIns;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dialogue
{
    public class DialogueManager : Singleton<DialogueManager>
    {
        [SerializeField] private GameObject _dialoguePanel;
        [SerializeField] private TextMeshProUGUI _dialogueText;
        [SerializeField] private GameObject[] _choices;
        
        private TextMeshProUGUI[] _choiceTexts;
        private Story _currentStory;
        private Controls _controls;

        private const float WaitTillCanMove = .2f;
        private bool _onDialogueIsActive = false;

        protected override void Awake()
        {
            base.Awake();
            
            _controls = new Controls();
            _onDialogueIsActive = false;
            _dialoguePanel.SetActive(false);
            _choiceTexts = new TextMeshProUGUI[_choices.Length];
            var index = 0;
            foreach (var choice in _choices)
            {
                _choiceTexts[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
                index++;
            }
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
            if (!_onDialogueIsActive) return;

            if (_controls.UI.Submit.triggered)
                ContinueDialogue();
        }
        

        public void EnterDialogueMode(TextAsset inkJson)
        {
            _currentStory = new Story(inkJson.text);
            _onDialogueIsActive = true;
            _dialoguePanel.SetActive(true);
            
            ContinueDialogue();
        }

        private void ContinueDialogue()
        {
            if (_currentStory.canContinue)
            {
                _dialogueText.text = _currentStory.Continue();
                DisplayChoices();
            }
            else
                StartCoroutine(ExitDialogueMode());
        }

        private void DisplayChoices()
        {
            var currentChoices = _currentStory.currentChoices;

            if (currentChoices.Count > _choices.Length)
                Debug.LogError($"Not enough choices in the UI. Number of choices in the UI: " +
                               $"{_choices.Length} Number of choices in the ink file: {currentChoices.Count}");

            // enable and initialize the choices in the UI
            var index = 0;
            foreach (var choice in currentChoices)
            {
                _choices[index].gameObject.SetActive(true);
                _choiceTexts[index].text = choice.text;
                index++;
            }
            
            // disable the rest of the choices
            for (var i = index; i < _choices.Length; i++)
                _choices[i].gameObject.SetActive(false);
            
            StartCoroutine(SelectChoice());
        }
        
        public void ChooseChoice(int choiceIndex)
        {
            _currentStory.ChooseChoiceIndex(choiceIndex);
        }
        
        public bool OnDialogueActive()
        {
            return _onDialogueIsActive;
        }

        private IEnumerator SelectChoice()
        {
            EventSystem.current.SetSelectedGameObject(null);
            yield return new WaitForEndOfFrame();
            EventSystem.current.SetSelectedGameObject(_choices[0].gameObject);
        }
        
        private IEnumerator ExitDialogueMode()
        {
            yield return new WaitForSeconds(WaitTillCanMove);
            _onDialogueIsActive = false;
            _dialoguePanel.SetActive(false);
            _dialogueText.text = string.Empty;
        }
    }
}