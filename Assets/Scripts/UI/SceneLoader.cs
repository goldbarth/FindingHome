using UnityEngine;
using UnityEngine.SceneManagement;

//TODO: unload scene when go to next scene
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
        public static SceneLoader Instance { get; private set; }
        [SerializeField] private SceneIndex startScene;

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
            
            LoadScene(startScene);
        }

        public void LoadScene(SceneIndex sceneIndex, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            SceneManager.LoadSceneAsync((int)sceneIndex, loadSceneMode);
        }
        
        public void UnloadScene(SceneIndex sceneIndex)
        {
        }
        
        public void UnloadCurrentScene()
        {
        }
    }
}