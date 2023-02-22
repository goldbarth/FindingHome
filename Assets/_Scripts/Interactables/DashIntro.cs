using System.Collections;
using Player;
using TMPro;
using UI;
using UnityEngine;

namespace Interactables
{
    public class DashIntro : Menu
    {
        [SerializeField] private GameObject introPanel;
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private string[] dialogue;
        [SerializeField] private float speed;
        [SerializeField] private bool playerInRange;
        [SerializeField] private GameObject button;

        private PlayerController2D _player;
        private int _index;

        private void Awake()
        {
            _player = FindObjectOfType<PlayerController2D>();
        }

        private void Update()
        {
            if (_player.IsInteracting && playerInRange)
            {
                if(introPanel.activeInHierarchy)
                    ZeroText();
                else
                {
                    introPanel.SetActive(true);
                    Time.timeScale = 0;
                    StartCoroutine(Typing());
                }
            }

            if (dialogueText.text == dialogue[_index])
            {
                button.SetActive(true);
            }
        }

        public void ZeroText()
        {
            dialogueText.text = "";
            _index = 0;
            introPanel.SetActive(false);
        }

        public void NextLine()
        {
            
            button.SetActive(false);
            if(_index < dialogue.Length - 1)
            {
                _index++;
                dialogueText.text = "";
                StartCoroutine(Typing());
            }
            else
            {
                ZeroText();
            }
        }
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player"))
            {
                playerInRange = true;
                introPanel.SetActive(true);
                //Time.timeScale = 0;
                Debug.Log("DashIntro");
            }
        }
        
        private void OnTriggerExit2D(Collider2D col)
        {
            if (col.CompareTag("Player"))
            {
                playerInRange = false;
                introPanel.SetActive(false);
                //Time.timeScale = 1;
            }
        }
        
        private IEnumerator Typing()
        {
            foreach (var letter in dialogue[_index].ToCharArray())
            {
                dialogueText.text += letter;
                yield return new WaitForSeconds(speed);
            }
        }
    }
}