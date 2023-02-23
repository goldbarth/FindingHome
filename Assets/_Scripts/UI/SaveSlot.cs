using DataPersistence;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace UI
{
    public class SaveSlot : MonoBehaviour
    {
        [Header("PROFILE")][SerializeField] private string profileId;
        
        [Space][Header("OBJECTS")]
        [SerializeField] private GameObject noDataContent;
        [Space][SerializeField] private GameObject hasDataContent;
        [Space][SerializeField] private TextMeshProUGUI percentageCompleteText;
        [Space][SerializeField] private TextMeshProUGUI deathCountText;
        
        [Space][Header("CLEAR DATA BUTTON")]
        [SerializeField] private Button deleteButton;
        
        public bool HasData { get; private set; }
        
        private Button _saveSlotButton;
        

        private void Awake()
        {
            _saveSlotButton = GetComponent<Button>();
        }

        public void SetData(GameData data)
        {
            if (data == null)
            {
                HasData = false;
                noDataContent.SetActive(true);
                hasDataContent. SetActive(false);
                deleteButton.gameObject.SetActive(false);
            }
            else
            {
                HasData = true;
                noDataContent.SetActive(false);
                hasDataContent. SetActive(true);
                deleteButton.gameObject.SetActive(true);

                percentageCompleteText.text = data.GetPercentageComplete() + "% COMPLETE";
                deathCountText.text = "DEATH COUNT: " + data.deathCount;
            }
        }

        public string GetProfileId()
        {
            return profileId;
        }
        
        public void SetInteractable(bool interactable)
        {
            _saveSlotButton.interactable = interactable;
            deleteButton.interactable = interactable;
        }
    }
}
