using Cinemachine;
using UnityEngine;

namespace AnimationHandler
{
    public class CinemachineShake : MonoBehaviour
    {
        // Delegates aren't working /bc on every room change, the delegate is reset
        // and for some reason, the delegate is not being called
        public static CinemachineShake Instance { get; private set; }
        
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;
        
        private CinemachineVirtualCamera _perlin;
        
        private readonly float _ampIntensity = 2.9f;
        private readonly float _freqIntensity = 2f;
        private readonly float _shakeTime = .2f;

        private void Awake()
        {
            Instance = this;
        }

        // had to use invoke instead of coroutines /bc camera changes on every room change
        // for some reason it works better with invoke
        public void CameraShake()
        {
            var perlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            perlin.m_AmplitudeGain = _ampIntensity;
            perlin.m_FrequencyGain = _freqIntensity;
            Invoke(nameof(StopShaking), _shakeTime);
        }
        
        private void StopShaking()
        {
            var perlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            perlin.m_AmplitudeGain = 0f;
            perlin.m_FrequencyGain = 0f;
        }
    }
}