using System;
using UnityEngine.EventSystems;
using DataPersistence;
using SceneHandler;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class OptionsMenu: Menu, IDataPersistence
    {
        //[SerializeField] private AudioMixer audioMixer;
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;

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
            //Debug.Log(masterVolumeSlider.value);
        }
        
        public void OnSFXSliderChanged(float value)
        {
            //Debug.Log(sfxVolumeSlider.value);
        }
        
        public void OnMusicSliderChanged(float value)
        {
            //Debug.Log(musicVolumeSlider.value);
        }
        
        public void OnBackButtonClicked()
        {
            DataPersistenceManager.Instance.SaveGame();
            SceneLoader.Instance.UnloadSceneAsync();
        }

        public void LoadData(GameData data)
        {
            if (data == null)
            {
                Debug.LogWarning("Audio LOAD data is null.");
                return;
            }
            
            masterVolumeSlider.value = data.masterVolume;
            sfxVolumeSlider.value = data.sfxVolume;
            musicVolumeSlider.value = data.musicVolume;
        }

        //TODO: audio mixer data (master, sfx, music)
        public void SaveData(GameData data)
        {
            if (data == null)
            {
                Debug.LogWarning("Audio SAVE data is null.");
                return;
            }
            
            data.masterVolume = masterVolumeSlider.value;
            data.sfxVolume = sfxVolumeSlider.value;
            data.musicVolume = musicVolumeSlider.value;
        }
    }
}