using SceneHandler;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace AddIns
{
    public class PauseManager : Singleton<PauseManager>, Controls.IUIActions
    {
        [Header("SCENE TO LOAD")]
        [SerializeField] private SceneIndices _pauseMenuScene;
        
        [Header("SCENE MODE")]
        [SerializeField] private LoadSceneMode _loadSceneMode = LoadSceneMode.Additive;

        private Controls _controls;

        protected override void Awake()
        {
            base.Awake();
            
            _controls = new Controls();
            _controls.UI.SetCallbacks(this);
        }

        private void OnEnable()
        {
            _controls.Enable();
        }
        
        private void OnDisable()
        {
            _controls.Disable();
        }

        public void OnSubmit(InputAction.CallbackContext context)
        { }

        public void OnMove(InputAction.CallbackContext context)
        { }

        public void OnPointer(InputAction.CallbackContext context)
        { }

        public void OnLeftClick(InputAction.CallbackContext context)
        { }

        public void OnPause(InputAction.CallbackContext context)
        {
            Debug.Log("Is Paused: " + GameManager.Instance.IsGamePaused);
            Debug.Log("Is Game Started: " + GameManager.Instance.IsGameStarted);
            if (context.started && !GameManager.Instance.IsGamePaused && GameManager.Instance.IsGameStarted)
                SceneLoader.Instance.LoadSceneAsync(_pauseMenuScene, _loadSceneMode);
        }
    }
}
