using DataPersistence;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class OptionsMenu: Menu, IDataPersistence
    {
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        
        private EventSystem _eventSystem;
        private PauseMenu _pauseMenu;

        private void Awake()
        {
            _eventSystem = FindObjectOfType<EventSystem>();
            _eventSystem.SetSelectedGameObject(masterVolumeSlider.gameObject);
            masterVolumeSlider.onValueChanged.AddListener(delegate { OnMasterSliderChanged(); });
            sfxVolumeSlider.onValueChanged.AddListener(delegate { OnMasterSliderChanged(); });
            musicVolumeSlider.onValueChanged.AddListener(delegate { OnMasterSliderChanged(); });
        }
        
        private void OnMasterSliderChanged()
        {
            var masterVolume = masterVolumeSlider.value;
            var sfxVolume = sfxVolumeSlider.value;
            var musicVolume = musicVolumeSlider.value;
            
            // Do something with the values
        }

        public void OnMasterSliderChanged(float value)
        {
            Debug.Log(masterVolumeSlider.value);
        }
        
        public void OnSFXSliderChanged(float value)
        {
            Debug.Log(sfxVolumeSlider.value);
        }
        
        public void OnMusicSliderChanged(float value)
        {
            Debug.Log(musicVolumeSlider.value);
        }
        
        public void OnBackButtonClicked()
        {
            SceneLoader.Instance.LoadScene(GameManager.Instance.IsPaused ? SceneIndex.PauseMenu : SceneIndex.MainMenu);
        }

        public void LoadData(GameData data)
        {
            masterVolumeSlider.value = data.masterVolume;
            sfxVolumeSlider.value = data.sfxVolume;
            musicVolumeSlider.value = data.musicVolume;
        }

        public void SaveData(GameData data)
        {
            data.masterVolume = masterVolumeSlider.value;
            data.sfxVolume = sfxVolumeSlider.value;
            data.musicVolume = musicVolumeSlider.value;
        }
    }
}