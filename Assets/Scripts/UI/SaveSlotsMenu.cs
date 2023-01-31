using DataPersistence;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class SaveSlotsMenu : Menu
    {
        [Header("MENU NAVIGATION")] 
        [SerializeField] private UI.MainMenu mainMenu;
        
        [Header("MENU BUTTONS")]
        [SerializeField] private Button backButton;
        
        [Header("CONFIRMATION POPUP")]
        [SerializeField] private ConfirmationPopupMenu confirmationPopupMenu;
        
        private SaveSlot[] _saveSlots;
        
        private bool _isLoadingGame = false;

        private void Awake()
        {
            _saveSlots = GetComponentsInChildren<SaveSlot>();
        }

        public void OnSaveSlotClicked(SaveSlot saveSlot)
        {
            DisableMenuButtons();

            if (_isLoadingGame)
            {
                // update the selected profile id to be used for data persistence operations
                DataPersistenceManager.Instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
                SaveGameAndLoadScene();
            }
            else if (saveSlot.HasData)
            {
                confirmationPopupMenu.ActivateMenu(
                    "Starting a new Game will override your current progress. Are you sure you want to continue?", () =>
                { //confirm button callback "yes"
                    DataPersistenceManager.Instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
                    DataPersistenceManager.Instance.NewGame();
                    SaveGameAndLoadScene();
                }, () =>
                { //cancel button callback "no"
                    ActivateMenu(_isLoadingGame);
                });
            }
            else // case new game - save slot is empty
            {
                DataPersistenceManager.Instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
                DataPersistenceManager.Instance.NewGame();
                SaveGameAndLoadScene();
            }
        }
        
        private static void SaveGameAndLoadScene()
        {
            // save the game anytime before loading a new scene
            DataPersistenceManager.Instance.SaveGame();
            SceneManager.LoadSceneAsync((int)SceneIndex.Game);
        }
        
        public void OnDeleteButtonClicked(SaveSlot saveSlot)
        {
            confirmationPopupMenu.ActivateMenu(
                "Starting a new Game will override your current progress. Are you sure you want to continue?", () =>
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
            mainMenu.ActivateMenu();
            DeactivateMenu();
        }

        public void ActivateMenu(bool isLoadingGame)
        {
            gameObject.SetActive(true);
            
            // set the flag to indicate if we are loading a game or starting a new one
            _isLoadingGame = isLoadingGame;
            
            // load all of the profiles that exist
            var profilesGameData = DataPersistenceManager.Instance.GetAllProfilesGameData();
            
            // enable back button when menu gets activated
            backButton.interactable = true;
            
            // loop through each save slot(button) in the UI and set the right buttons active/inactive
            var firstSelected = backButton.gameObject;
            foreach (var saveSlot in _saveSlots)
            {
                GameData profileData = null;
                profilesGameData.TryGetValue(saveSlot.GetProfileId(), out profileData);
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
                    {
                        firstSelected = saveSlot.gameObject;
                    }
                }
            }
            
            var firstSelectedButton = firstSelected.GetComponent<Button>();
            SetFirstSelected(firstSelectedButton);
        }
        
        private void DeactivateMenu()
        {
            gameObject.SetActive(false);
        }
        
        private void DisableMenuButtons()
        {
            foreach (var saveSlot in _saveSlots)
            {
                saveSlot.SetInteractable(false);
            }
            
            backButton.interactable = false;
        }
    }
}
