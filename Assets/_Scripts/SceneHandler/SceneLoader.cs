using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        OptionsMenu,
        PauseMenu,
        LoadMenu,
        Level1,
        Level2,
    }

    public class SceneLoader : Singleton<SceneLoader>
    {
        [Header("SCENE LOADER")] [SerializeField]
        private SceneIndex startScene;

        [Space] [Header("LOADING SCREEN")] [SerializeField]
        private float minLoadingDuration;

        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private Image progressBarFill;

        private readonly LinkedList<Enum> _sceneNames = new();

        protected override void Awake()
        {
            base.Awake();
            LoadSceneAsync(startScene);
            loadingScreen.SetActive(false);

            SceneManager.sceneLoaded += RegisterNewScene;
            SceneManager.sceneUnloaded += UnregisterScene;
        }

        private void Update()
        {
            var result = _sceneNames.Aggregate("List contents: ", (current, item) => current + (item + ", "));
            Debug.Log(result);
        }

        private void RegisterNewScene(Scene scene, LoadSceneMode mode)
        {
            _sceneNames.AddLast((SceneIndex)scene.buildIndex);
        }

        private void UnregisterScene(Scene scene)
        {
            _sceneNames.Remove((SceneIndex)scene.buildIndex);
        }

        public void LoadSceneAsync(SceneIndex sceneIndex, LoadSceneMode loadSceneMode = LoadSceneMode.Single,
            bool showProgress = false)
        {
            var asyncOperation = SceneManager.LoadSceneAsync((int)sceneIndex, loadSceneMode);
            if (showProgress) StartCoroutine(LoadingProgress(asyncOperation));
        }

        public void UnloadSceneAsync()
        {
            SceneManager.UnloadSceneAsync(Convert.ToInt32(_sceneNames.First.Value));
            _sceneNames.RemoveLast();
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