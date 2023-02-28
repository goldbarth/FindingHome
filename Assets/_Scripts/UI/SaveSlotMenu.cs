using DataPersistence;
using SceneHandler;
using UnityEngine.UI;
using UnityEngine;

namespace UI
{
    public class SaveSlotMenu : Menu
    {
        private MainMenu _mainMenu;
        
        [Header("MENU BUTTONS")]
        [SerializeField] private Button backButton;
        [Header("CONFIRMATION POPUP")]
        [SerializeField] private ConfirmationPopupMenu confirmationPopupMenu;
        [Header("SCENE TO LOAD")]
        [SerializeField] private SceneIndices sceneToLoad;
        
        private SaveSlot[] _saveSlots;
        
        private bool _isLoadingGame;
        private bool _isNewGame;

        private void Awake()
        {
            _saveSlots = GetComponentsInChildren<SaveSlot>();
            _mainMenu = FindObjectOfType<MainMenu>();
            
            if (GameManager.Instance.IsGamePaused)
                GameManager.Instance.IsPauseMenuActive = false;
        }

        private void Start()
        {
            if (GameManager.Instance.IsNewGame) ActivateSaveSlots(false);
            else if (!GameManager.Instance.IsNewGame) ActivateSaveSlots(true);
            else if (GameManager.Instance.IsGamePaused) ActivateSaveSlots(true);
        }

        public void OnSaveSlotClicked(SaveSlot saveSlot)
        {
            DisableSaveSlotsWhenEmpty();

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
                    ActivateSaveSlots(_isLoadingGame);
                });
            }
            else // case new game - save slot is empty
            {
                DataPersistenceManager.Instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
                DataPersistenceManager.Instance.NewGame();
                LoadSceneSaveGame();
            }
        }
        
        private void LoadSceneSaveGame()
        {
            GameManager.Instance.IsGameStarted = true;
            DataPersistenceManager.Instance.SaveGame();
            SceneLoader.Instance.LoadSceneAsync(sceneToLoad, showProgress: true);
            if (GameManager.Instance.IsGamePaused) GameManager.Instance.IsGamePaused = false;
        }
        
        public void OnDeleteButtonClicked(SaveSlot saveSlot)
        {
            confirmationPopupMenu.ActivateMenu(
                "Do you want delete your current progress?", () =>
                { //confirm button callback "yes"
                    DataPersistenceManager.Instance.DeleteProfileData(saveSlot.GetProfileId());
                    ActivateSaveSlots(_isLoadingGame);
                }, () =>
                { //cancel button callback "no"
                    ActivateSaveSlots(_isLoadingGame);
                });
        }

        public void OnBackButtonClicked()
        {
            if (_mainMenu != null) _mainMenu.DisableButtonsDependingOnData();
            SceneLoader.Instance.UnloadSceneAsync();
        }

        private void OnDestroy()
        {
            if (!GameManager.Instance.IsGamePaused)
                GameManager.Instance.IsMenuActive = true;
            
            GameManager.Instance.IsSelected = false;
            if (GameManager.Instance.IsGamePaused)
                GameManager.Instance.IsPauseMenuActive = true;
            
        }

        private void ActivateSaveSlots(bool isLoadingGame)
        {
            // set the flag to indicate if a game is loading or starting a new one
            _isLoadingGame = isLoadingGame;
            
            // load all of the profiles that exist
            var profilesGameData = DataPersistenceManager.Instance.GetAllProfilesGameData();

            // loop through each save slot(button) and compare with the existing profiles,
            // to set the right buttons active/inactive in the UI
            var firstSelected = backButton.gameObject;
            foreach (var saveSlot in _saveSlots)
            {
                profilesGameData.TryGetValue(saveSlot.GetProfileId(), out var profileData);

                saveSlot.SetData(profileData);

                if (profileData == null && isLoadingGame)
                    saveSlot.SetInteractable(false);
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
        
        private void DisableSaveSlotsWhenEmpty()
        {
            foreach (var saveSlot in _saveSlots)
                saveSlot.SetInteractable(false);

            backButton.interactable = false;
        }
    }
}
