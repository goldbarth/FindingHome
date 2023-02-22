using DataPersistence;
using SceneHandler;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class SaveSlotsMenu : Menu
    {
        private MainMenu _mainMenu;
        
        [Header("MENU BUTTONS")]
        [SerializeField] private Button backButton;
        [Header("CONFIRMATION POPUP")]
        [SerializeField] private ConfirmationPopupMenu confirmationPopupMenu;
        
        private SaveSlot[] _saveSlots;
        
        private bool _isLoadingGame;
        private bool _isNewGame;

        private void Awake()
        {
            _saveSlots = GetComponentsInChildren<SaveSlot>();
            _mainMenu = FindObjectOfType<MainMenu>();
        }

        private void Start()
        {
            if (GameManager.Instance.IsNewGame) ActivateMenu(false);
            else if (!GameManager.Instance.IsNewGame) ActivateMenu(true);
        }

        public void OnSaveSlotClicked(SaveSlot saveSlot)
        {
            DisableMenuButtons();

            if (_isLoadingGame)
            {
                // update the selected profile id to be used for data persistence
                DataPersistenceManager.Instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
                LoadSceneSaveGame();
            }
            else if (saveSlot.HasData)
            {
                confirmationPopupMenu.ActivateMenu(
                    "Starting a new Game will override your current progress. Are you sure you want to continue?", () =>
                { //confirm button callback "yes"
                    DataPersistenceManager.Instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
                    DataPersistenceManager.Instance.NewGame();
                    LoadSceneSaveGame();
                }, () =>
                { //cancel button callback "no"
                    ActivateMenu(_isLoadingGame);
                });
            }
            else // case new game - save slot is empty
            {
                DataPersistenceManager.Instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
                DataPersistenceManager.Instance.NewGame();
                LoadSceneSaveGame();
            }
        }
        
        private static void LoadSceneSaveGame()
        {
            DataPersistenceManager.Instance.SaveGame();
            SceneLoader.Instance.LoadSceneAsync(SceneIndex.Level1, showProgress: true);
            if (GameManager.Instance.IsPaused) GameManager.Instance.IsPaused = false;
        }
        
        public void OnDeleteButtonClicked(SaveSlot saveSlot)
        {
            confirmationPopupMenu.ActivateMenu(
                "Do you want delete your current progress?", () =>
                { //confirm button callback "yes"
                    DataPersistenceManager.Instance.DeleteProfileData(saveSlot.GetProfileId());
                    ActivateMenu(_isLoadingGame);
                }, () =>
                { //cancel button callback "no"
                    ActivateMenu(_isLoadingGame);
                });
        }

        public void OnBackButtonClicked()
        {
            if (_mainMenu != null) _mainMenu.DisableButtonsDependingOnData();
            SceneLoader.Instance.LoadSceneAsync(GameManager.Instance.IsPaused 
                ? SceneIndex.PauseMenu : SceneIndex.MainMenu, 
                GameManager.Instance.IsPaused ? LoadSceneMode.Additive : LoadSceneMode.Single);
            
                SceneLoader.Instance.UnloadSceneAsync();
        }

        private void ActivateMenu(bool isLoadingGame)
        {
            // set the flag to indicate if a game is loading or starting a new one
            _isLoadingGame = isLoadingGame;
            
            // load all of the profiles that exist
            var profilesGameData = DataPersistenceManager.Instance.GetAllProfilesGameData();

            // loop through each save slot(button) in the UI and set the right buttons active/inactive
            var firstSelected = backButton.gameObject;
            foreach (var saveSlot in _saveSlots)
            {
                profilesGameData.TryGetValue(saveSlot.GetProfileId(), out var profileData);
                saveSlot.SetData(profileData);

                if (profileData == null && isLoadingGame)
                {
                    saveSlot.SetInteractable(false);
                }
                else
                {
                    saveSlot.SetInteractable(true);
                    
                    // set the first selectable button to be the first save slot that is interactable
                    if (firstSelected.Equals(backButton.gameObject))
                        firstSelected = saveSlot.gameObject;
                }
            }
            
            var firstSelectedButton = firstSelected.GetComponent<Button>();
            SetFirstSelected(firstSelectedButton);
        }
        
        private void DisableMenuButtons()
        {
            foreach (var saveSlot in _saveSlots)
                saveSlot.SetInteractable(false);

            backButton.interactable = false;
        }
    }
}
