using DataPersistence;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace UI
{
    public class SaveSlot : MonoBehaviour
    {
        [Header("PROFILE")]
        [SerializeField] private string _profileId = "save1";
        
        [Space][Header("OBJECTS")]
        [SerializeField] private GameObject _noDataContent;
        [Space][SerializeField] private GameObject _hasDataContent;
        [Space][SerializeField] private TextMeshProUGUI _percentageCompleteText;
        [Space][SerializeField] private TextMeshProUGUI _deathCountText;
        
        [Space][Header("DELETE BUTTON")]
        [SerializeField] private Button _deleteButton;
        
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
                _noDataContent.SetActive(true);
                _hasDataContent. SetActive(false);
                _deleteButton.gameObject.SetActive(false);
            }
            else
            {
                HasData = true;
                _noDataContent.SetActive(false);
                _hasDataContent. SetActive(true);
                _deleteButton.gameObject.SetActive(true);

                _percentageCompleteText.text = data.GetPercentageComplete() + "% TOTAL";
                _deathCountText.text = "DEATH COUNT: " + data.deathCount;
            }
        }

        public string GetProfileId()
        {
            return _profileId;
        }
        
        public void SetInteractable(bool interactable)
        {
            _saveSlotButton.interactable = interactable;
            _deleteButton.interactable = interactable;
        }
    }
}
