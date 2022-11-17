using UnityEngine.SceneManagement;

namespace Core.SceneLoading
{
    public class SceneLoader : ISceneLoader
    {
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}