using Audio;
using DataPersistence;
using SceneHandler;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Audio;

namespace UI
{
    public class OptionsMenu: Menu
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
            DataPersistenceManager.Instance.SaveAudioProfile();
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
    }
}