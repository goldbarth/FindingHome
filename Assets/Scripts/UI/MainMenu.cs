using DataPersistence;
using SceneHandler;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            SceneLoader.Instance.LoadSceneAsync(SceneIndex.LoadMenu);
            GameManager.Instance.IsNewGame = true;
        }
        
        public void OnLoadGameClicked()
        {
            SceneLoader.Instance.LoadSceneAsync(SceneIndex.LoadMenu, LoadSceneMode.Additive);
            GameManager.Instance.IsNewGame = false;
        }

        public void OnContinueGameClicked()
        {
            SceneLoader.Instance.LoadSceneAsync(SceneIndex.Game, showProgress: true);
        }
        
        public void OnOptionsClicked()
        {
            SceneLoader.Instance.LoadSceneAsync(SceneIndex.OptionsMenu, LoadSceneMode.Additive);
        }
        
        public void OnExitClicked()
        {
            Application.Quit();
        }
        
        #endregion
    }
}