using UnityEngine.Audio;
using DataPersistence;
using UnityEngine.UI;
using SceneHandler;
using UnityEngine;

namespace UI
{
    public class OptionsMenu: Menu
    {
        [Header("AUDIO COMPONENTS")]
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private Slider _masterVolumeSlider;
        [SerializeField] private Slider _sfxVolumeSlider;
        [SerializeField] private Slider _musicVolumeSlider;
        [Space][Header("AUDIO PARAMETERS")]
        [SerializeField] private string _masterVolumeParameter = "Master";
        [SerializeField] private string _sfxVolumeParameter = "SFX";
        [SerializeField] private string _musicVolumeParameter = "Music";
        [Space][Header("AUDIO MULTIPLIER")]
        [SerializeField] private float _multiplier = 30f;
        [Space][Header("BACKGROUNDS")]
        [SerializeField] private GameObject _parallaxBackground;
        
        private void Awake()
        {
            _parallaxBackground.SetActive(!GameManager.Instance.IsGamePaused);
            if (GameManager.Instance.IsGamePaused)
                GameManager.Instance.IsPauseMenuActive = false;
            _masterVolumeSlider.onValueChanged.AddListener(OnMasterSliderChanged);
            _sfxVolumeSlider.onValueChanged.AddListener(OnSFXSliderChanged);
            _musicVolumeSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        }

        public void OnMasterSliderChanged(float value)
        {
            _audioMixer.SetFloat(_masterVolumeParameter, Mathf.Log10(value) * _multiplier);
        }
        
        public void OnSFXSliderChanged(float value)
        {
            _audioMixer.SetFloat(_sfxVolumeParameter, Mathf.Log10(value) * _multiplier);
        }
        
        public void OnMusicSliderChanged(float value)
        {
            _audioMixer.SetFloat(_musicVolumeParameter, Mathf.Log10(value) * _multiplier);
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