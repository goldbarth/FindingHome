﻿using System.Collections;
using AddIns;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dialogue
{
    public class DialogueManager : Singleton<DialogueManager>
    {
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private TextMeshProUGUI dialogueText;
        
        [SerializeField] private GameObject[] choices;
        
        private Controls _controls;
        private Story _currentStory;
        private TextMeshProUGUI[] _choiceTexts;
        
        private readonly float _waitTillCanMove = .2f;
        public bool OnDialogueIsActive {get; private set;} = false;

        protected override void Awake()
        {
            base.Awake();
            
            _controls = new Controls();
            OnDialogueIsActive = false;
            dialoguePanel.SetActive(false);
            _choiceTexts = new TextMeshProUGUI[choices.Length];
            var index = 0;
            foreach (var choice in choices)
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
            if (!OnDialogueIsActive) return;

            if (_controls.UI.Submit.triggered)
                ContinueDialogue();
        }
        

        public void EnterDialogueMode(TextAsset inkJson)
        {
            _currentStory = new Story(inkJson.text);
            OnDialogueIsActive = true;
            dialoguePanel.SetActive(true);
            
            ContinueDialogue();
        }

        private IEnumerator ExitDialogueMode()
        {
            yield return new WaitForSeconds(_waitTillCanMove);
            OnDialogueIsActive = false;
            dialoguePanel.SetActive(false);
            dialogueText.text = "";
        }
        
        private void ContinueDialogue()
        {
            if (_currentStory.canContinue)
            {
                dialogueText.text = _currentStory.Continue();
                DisplayChoices();
            }
            else
                StartCoroutine(ExitDialogueMode());
        }

        private void DisplayChoices()
        {
            var currentChoices = _currentStory.currentChoices;

            if (currentChoices.Count > choices.Length)
                Debug.LogError($"Not enough choices in the UI. Number of choices in the UI: " +
                               $"{choices.Length} Number of choices in the ink file: {currentChoices.Count}");

            // enable and initialize the choices in the UI
            var index = 0;
            foreach (var choice in currentChoices)
            {
                choices[index].gameObject.SetActive(true);
                _choiceTexts[index].text = choice.text;
                index++;
            }
            
            // disable the rest of the choices
            for (var i = index; i < choices.Length; i++)
                choices[i].gameObject.SetActive(false);
            
            StartCoroutine(SelectChoice());
        }
        
        public void ChooseChoice(int choiceIndex)
        {
            _currentStory.ChooseChoiceIndex(choiceIndex);
        }
        
        private IEnumerator SelectChoice()
        {
            EventSystem.current.SetSelectedGameObject(null);
            yield return new WaitForEndOfFrame();
            EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
        }
    }
}