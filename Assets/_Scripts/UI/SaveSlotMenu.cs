using DataPersistence;
using UnityEngine.UI;
using SceneHandler;
using UnityEngine;

namespace UI
{
    public class SaveSlotMenu : Menu
    {
        private MainMenu _mainMenu;
        
        [Header("MENU BUTTONS")]
        [SerializeField] private Button[] _deleteButtons;
        [SerializeField] private Button _backButton;
        [Header("CONFIRMATION POPUP")]
        [SerializeField] private ConfirmationPopupMenu _confirmationPopupMenu;
        [Space][Header("PARALLAX BACKGROUND")]
        [Space][SerializeField] private GameObject _background;
        [Header("SCENE TO LOAD")]
        [SerializeField] private SceneIndices _sceneToLoad;
        
        private SaveSlot[] _saveSlots;
        
        private readonly string _overrideConfirmText = "Starting a new Game will override your current progress. Are you sure you want to continue?";
        private readonly string _deleteConfirmText = "Are you sure you want to delete your current progress?";
        private bool _isLoadGame;
        private bool _isNewGame;

        private void Awake()
        {
            GameManager.Instance.IsSaveSlotMenuActive = true;
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

        private void Update()
        {
            _background.SetActive(!GameManager.Instance.IsGamePaused);
            // foreach (var deleteButton in _deleteButtons)
            //     deleteButton.gameObject.SetActive(!GameManager.Instance.IsGamePaused);
        }

        public void OnSaveSlotClicked(SaveSlot saveSlot)
        {
            DisableSaveSlotsWhenEmpty();

            if (_isLoadGame)
            {
                // update the selected profile id to be used for data persistence
                DataPersistenceManager.Instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
                LoadSceneSaveGame();
            }
            else if (saveSlot.HasData)
            {
                _confirmationPopupMenu.ActivateMenu(
                    _overrideConfirmText, () =>
                { //confirm button callback "yes"
                    DataPersistenceManager.Instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
                    DataPersistenceManager.Instance.NewGame();
                    LoadSceneSaveGame();
                }, () =>
                { //cancel button callback "no"
                    ActivateSaveSlots(_isLoadGame);
                });
            }
            else // case new game - save slot is empty
            {
                DataPersistenceManager.Instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
                DataPersistenceManager.Instance.NewGame();
                LoadSceneSaveGame();
            }
            
            GameManager.Instance.IsSaveSlotMenuActive = false;
        }
        
        private void LoadSceneSaveGame()
        {
            GameManager.Instance.IsSaveSlotMenuActive = false;
            GameManager.Instance.IsGameStarted = true;
            DataPersistenceManager.Instance.SaveGame();
            SceneLoader.Instance.LoadSceneAsync(_sceneToLoad, showProgress: true);
            if (GameManager.Instance.IsGamePaused) GameManager.Instance.IsGamePaused = false;
        }
        
        public void OnDeleteButtonClicked(SaveSlot saveSlot)
        {
            _confirmationPopupMenu.ActivateMenu(
                _deleteConfirmText, () =>
                { //confirm button callback "yes"
                    GameManager.Instance.IsSaveSlotMenuActive = false;
                    DataPersistenceManager.Instance.DeleteProfileData(saveSlot.GetProfileId());
                    ActivateSaveSlots(_isLoadGame);
                }, () =>
                { //cancel button callback "no"
                    ActivateSaveSlots(_isLoadGame);
                });
        }

        public void OnBackButtonClicked()
        {
            GameManager.Instance.IsSaveSlotMenuActive = false;
            if (_mainMenu != null) _mainMenu.DisableButtonsDependingOnData();
            SceneLoader.Instance.UnloadSceneAsync();
        }

        private void OnDestroy()
        {
            GameManager.Instance.IsSaveSlotMenuActive = false;
            if (!GameManager.Instance.IsGamePaused)
                GameManager.Instance.IsMenuActive = true;
            
            GameManager.Instance.IsSelected = false;
            if (GameManager.Instance.IsGamePaused)
                GameManager.Instance.IsPauseMenuActive = true;
            
        }

        private void ActivateSaveSlots(bool isLoadGame)
        {
            // set the flag to indicate if a game will be loaded or starting a new one
            _isLoadGame = isLoadGame;
            
            // load all of the profiles that exist
            var profilesGameData = DataPersistenceManager.Instance.GetAllProfilesGameData();

            // loop through each save slot(button) and compare with the existing profiles,
            // to set the right buttons active/inactive in the UI
            var firstSelected = _backButton.gameObject;
            foreach (var saveSlot in _saveSlots)
            {
                profilesGameData.TryGetValue(saveSlot.GetProfileId(), out var profileData);

                saveSlot.SetData(profileData);

                if (profileData == null && isLoadGame)
                {
                    saveSlot.SetInteractable(false);
                    Debug.Log("Skipping directory, all files are loaded or there are no existing file.");
                }
                else
                {
                    saveSlot.SetInteractable(true);
                    
                    // set the first selectable button to be the first save slot that is interactable
                    if (firstSelected.Equals(_backButton.gameObject))
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

            _backButton.interactable = false;
        }
    }
}
