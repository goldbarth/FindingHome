using System;
using System.Collections.Generic;
using DataPersistence;
using UnityEngine;

namespace MainMenu
{
    public class SaveSlotsMenu : MonoBehaviour
    {
        private SaveSlot[] _saveSlots;

        private void Awake()
        {
            _saveSlots = GetComponentsInChildren<SaveSlot>();
        }

        private void Start()
        {
            ActiveMenu();
        }

        public void ActiveMenu()
        {
            // load all of the profiles that exist
            var profilesGameData = DataPersistenceManager.Instance.GetAllProfilesGameData();
            
            // loop through each save slot in the UI and set the content appropriately
            foreach (var saveSlot in _saveSlots)
            {
                GameData profileData = null;
                profilesGameData.TryGetValue(saveSlot.GetProfileId(), out profileData);
                saveSlot.SetData(profileData);
            }
        }
    }
}
