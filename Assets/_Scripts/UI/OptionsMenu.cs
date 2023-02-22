using System;
using UnityEngine.EventSystems;
using DataPersistence;
using SceneHandler;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Audio;

namespace UI
{
    public class OptionsMenu: Menu, IDataPersistence
    {
        
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private string masterVolumeParameter = "Master";
        [SerializeField] private string sfxVolumeParameter = "SFX";
        [SerializeField] private string musicVolumeParameter = "Music";
        [SerializeField] private float multiplier = 20f;


        private void Awake()
        {
            var eventSystem = FindObjectOfType<EventSystem>();
            eventSystem.SetSelectedGameObject(masterVolumeSlider.gameObject);
            masterVolumeSlider.onValueChanged.AddListener(OnMasterSliderChanged);
            sfxVolumeSlider.onValueChanged.AddListener(OnMasterSliderChanged);
            musicVolumeSlider.onValueChanged.AddListener(OnMasterSliderChanged);
        }

        public void OnMasterSliderChanged(float value)
        {
            audioMixer.SetFloat(masterVolumeParameter, Mathf.Log10(value) * multiplier);
        }
        
        public void OnSFXSliderChanged(float value)
        {
            audioMixer.SetFloat(sfxVolumeParameter, Mathf.Log10(value) * multiplier);
        }
        
        public void OnMusicSliderChanged(float value)
        {
            audioMixer.SetFloat(musicVolumeParameter, Mathf.Log10(value) * multiplier);
        }
        
        public void OnBackButtonClicked()
        {
            DataPersistenceManager.Instance.SaveGame();
            SceneLoader.Instance.LoadSceneAsync(SceneIndex.MainMenu);
            SceneLoader.Instance.UnloadSceneAsync();
        }

        public void LoadData(GameData data)
        {
            if (data == null)
            {
                Debug.LogWarning("Audio LOAD data is null.");
                return;
            }
            
            Debug.Log("Loading audio data...");
            masterVolumeSlider.value = data.masterVolume;
            sfxVolumeSlider.value = data.sfxVolume;
            musicVolumeSlider.value = data.musicVolume;
        }
        
        public void SaveData(GameData data)
        {
            if (data == null)
            {
                Debug.LogWarning("Audio SAVE data is null.");
                return;
            }
            
            Debug.Log("Saving audio data...");
            data.masterVolume = masterVolumeSlider.value;
            data.sfxVolume = sfxVolumeSlider.value;
            data.musicVolume = musicVolumeSlider.value;
        }
    }
}