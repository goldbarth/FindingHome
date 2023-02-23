using System;
using DataPersistence;
using SceneHandler;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace UI
{
    public class OptionsMenu: Menu, IDataPersistence
    {
        [Header("AUDIO COMPONENTS")]
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [Space][Header("AUDIO PARAMETERS")]
        [SerializeField] private string masterVolumeParameter = "Master";
        [SerializeField] private string sfxVolumeParameter = "SFX";
        [SerializeField] private string musicVolumeParameter = "Music";
        [Space][Header("AUDIO MULTIPLIER")]
        [SerializeField] private float multiplier = 20f;
        
        private void Awake()
        {
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
            SceneLoader.Instance.LoadSceneAsync(GameManager.Instance.IsPaused ? SceneIndex.PauseMenu : SceneIndex.MainMenu, 
                GameManager.Instance.IsPaused ? LoadSceneMode.Additive : LoadSceneMode.Single);
            SceneLoader.Instance.UnloadSceneAsync();
        }

        public void LoadData(GameData data)
        {
            try // fancy try catch block
            {
                if (data == null)
                    throw new ArgumentNullException(paramName: nameof(data), message: "Parameter can't be null.");
            
                Debug.Log("Loading audio data...");
                masterVolumeSlider.value = data.masterVolume;
                sfxVolumeSlider.value = data.sfxVolume;
                musicVolumeSlider.value = data.musicVolume;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"{e}\nAudio data couldn't be loaded. There is no save file to to read.");
            }
        }
        
        public void SaveData(GameData data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException(paramName: nameof(data), message: "Parameter can't be null.");

                Debug.Log("Saving audio data...");
                data.masterVolume = masterVolumeSlider.value;
                data.sfxVolume = sfxVolumeSlider.value;
                data.musicVolume = musicVolumeSlider.value;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"{e}\nAudio data couldn't be saved. There is no save file to to write.");
            }
        }
    }
}