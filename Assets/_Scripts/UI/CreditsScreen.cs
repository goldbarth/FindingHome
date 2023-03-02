using SceneHandler;

namespace UI
{
    public class CreditsScreen : Menu
    {
        public void OnBackButtonClicked()
        {
            SceneLoader.Instance.UnloadSceneAsync();
        }
        
        private void OnDestroy()
        {
            GameManager.Instance.IsMenuActive = true;
            GameManager.Instance.IsSelected = false;
        }
    }
}