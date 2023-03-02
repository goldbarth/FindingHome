using SceneHandler;

namespace UI
{
    public class ExitScreen : Menu
    {
        public void OnMenuButtonClicked()
        {
            SceneLoader.Instance.LoadSceneAsync(SceneIndices.MainMenu);
        }
    }
}