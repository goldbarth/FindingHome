using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace UI
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
    
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance;
        
        [SerializeField] private SceneIndex startScene;
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private float minLoadingDuration;
        [SerializeField] private Image progressBarFill;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
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