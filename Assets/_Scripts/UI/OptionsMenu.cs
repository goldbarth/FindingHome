using System;
using DataPersistence;
using SceneHandler;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Audio;

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
        [SerializeField] private float multiplier = 30f;
        [Space][Header("BACKGROUNDS")]
        [SerializeField] private GameObject parallaxBackground;

        private void Awake()
        {
            parallaxBackground.SetActive(!GameManager.Instance.IsGamePaused);
            if (GameManager.Instance.IsGamePaused)
                GameManager.Instance.IsPauseMenuActive = false;
            
            masterVolumeSlider.onValueChanged.AddListener(OnMasterSliderChanged);
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXSliderChanged);
            musicVolumeSlider.onValueChanged.AddListener(OnMusicSliderChanged);
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
            SceneLoader.Instance.UnloadSceneAsync();
        }

        private void OnDestroy()
        {
            if (!GameManager.Instance.IsGamePaused)
                GameManager.Instance.IsMenuActive = true;
            
            GameManager.Instance.IsSelected = false;
            if (GameManager.Instance.IsGamePaused)
                GameManager.Instance.IsPauseMenuActive = true;
        }

        public void LoadData(GameData data)
        {
            try // fancy try catch block
            {
                if (data == null)
                    throw new ArgumentNullException(paramName: nameof(data), message: "Load-Data can't be null.");
                Debug.LogWarning("OPTIONS: " + data.musicVolume);
                masterVolumeSlider.value = data.masterVolume;
                sfxVolumeSlider.value = data.sfxVolume;
                musicVolumeSlider.value = data.musicVolume;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"{e}\nAudio data couldn't be loaded. There is no save file to to read.");
            }
            finally
            {
                Debug.Log("Audio data loaded in Options-Menu.");
            }
        }
        
        public void SaveData(GameData data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException(paramName: nameof(data), message: "Save-Data can't be null.");
                
                data.masterVolume = masterVolumeSlider.value;
                data.sfxVolume = sfxVolumeSlider.value;
                data.musicVolume = musicVolumeSlider.value;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"{e}\nAudio data couldn't be saved. There is no save file to to write.");
            }
            finally
            {
                Debug.Log("Audio data saved in Options-Menu.");
            }
        }
    }
}