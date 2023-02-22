using System;
using Cinemachine;
using UnityEngine;

namespace AnimationHandler
{
    public class CinemachineShake : MonoBehaviour
    {
        public static CinemachineShake Instance { get; private set; }
        [SerializeField] private CinemachineVirtualCamera virtualCamera;

        private void Awake()
        {
            Instance = this;
        }

        public void CameraShake(float ampIntensity, float freqIntensity, float time)
        {
            var perlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            perlin.m_AmplitudeGain = ampIntensity;
            perlin.m_FrequencyGain = freqIntensity;
            Invoke(nameof(StopShaking), time);
        }
        
        private void StopShaking()
        {
            var perlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            perlin.m_AmplitudeGain = 0f;
            perlin.m_FrequencyGain = 0f;
        }
    }
}