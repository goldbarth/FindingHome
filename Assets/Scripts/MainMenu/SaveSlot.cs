using DataPersistence;
using TMPro;
using UnityEngine;

namespace MainMenu
{
    public class SaveSlot : MonoBehaviour
    {
        [Header("Profile")] [SerializeField] private string profileId;
        [Space][Header("Content")] [SerializeField] private GameObject noDataContent;
        [SerializeField] private GameObject hasDataContent;
        [SerializeField] private TextMeshProUGUI percentageCompleteText;
        [SerializeField] private TextMeshProUGUI deathCountText;

        public void SetData(GameData data)
        {
            if (data == null)
            {
                noDataContent.SetActive(true);
                hasDataContent. SetActive(false);
            }
            else
            {
                noDataContent.SetActive(false);
                hasDataContent. SetActive(true);

                percentageCompleteText.text = data.GetPercentageComplete() + "% COMPLETE";
                deathCountText.text = "DEATH COUNT" + data.deathCount;
            }
        }

        public string GetProfileId()
        {
            return profileId;
        }
    }
}
