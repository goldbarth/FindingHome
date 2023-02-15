using System.Collections;
using AddIns;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SceneHandler
{
    public enum SceneIndex
    {
        Init,
        MainMenu,
        Options,
        Game,
        PauseMenu,
        LoadMenu
    }
    
    public class SceneLoader : Singleton<SceneLoader>
    {
        [SerializeField] private SceneIndex startScene;
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private float minLoadingDuration;
        [SerializeField] private Image progressBarFill;

        protected override void Awake()
        {
            base.Awake();
            LoadSceneAsync(startScene);
            loadingScreen.SetActive(false);
        }

        public void LoadSceneAsync(SceneIndex sceneIndex, LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool showProgress = false)
        {
            var asyncOperation = SceneManager.LoadSceneAsync((int)sceneIndex, loadSceneMode);
            
            if (showProgress) StartCoroutine(LoadingProgress(asyncOperation));
        }
        
        private IEnumerator LoadingProgress(AsyncOperation asyncOperation)
        {
            loadingScreen.SetActive(true);
            progressBarFill.fillAmount = 0f;
            var counter = 0f;
            while (!asyncOperation.isDone || counter <= minLoadingDuration)
            {
                yield return null;
                counter += Time.deltaTime;

                var waitProgress = counter / minLoadingDuration;
                
                progressBarFill.fillAmount = Mathf.Min(asyncOperation.progress, waitProgress);
            }

            progressBarFill.fillAmount = 1f;
            loadingScreen.SetActive(false);
        }
    }
}