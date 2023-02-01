using System;
using DataPersistence;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainMenu : Menu
    {
        [Space][Header("MENU BUTTONS")]
        [Space][SerializeField] private Button continueGameButton;
        [Space][SerializeField] private Button loadGameButton;

        private void Start()
        {
            DisableButtonsDependingOnData();
        }

        public void DisableButtonsDependingOnData()
        {
            // Check if there is a save file and enable the continue button if there is
            if (DataPersistenceManager.Instance.HasGameData()) return;
            continueGameButton.interactable = false;
            loadGameButton.interactable = false;
        }

        #region Button Clicks

        public void OnNewGameClicked()
        {
            SceneLoader.Instance.LoadScene(SceneIndex.LoadMenu);
            GameManager.Instance.IsNewGame = true;
        }
        
        public void OnLoadGameClicked()
        {
            SceneLoader.Instance.LoadScene(SceneIndex.LoadMenu);
            GameManager.Instance.IsNewGame = false;
        }

        public void OnContinueGameClicked()
        {
            // save the game anytime before loading a new scene
            DataPersistenceManager.Instance.SaveGame();
            SceneLoader.Instance.LoadScene(SceneIndex.Game);
        }
        
        public void OnOptionsClicked()
        {
            SceneLoader.Instance.LoadScene(SceneIndex.Options);
        }
        
        public void OnExitClicked()
        {
            Application.Quit();
        }
        
        #endregion
    }
}