using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Code.UI;
namespace Code.LevelSystem
{
    public static class SceneLoader
    {
        #region Public Methods
        public static async void LoadScene(string sceneName, LoadSceneMode mode)
        {
            await LoadingScreenUI.Singleton.FadeIn(true);
            var asyncOp = SceneManager.LoadSceneAsync(sceneName, mode);
            while (!asyncOp.isDone)
            {
                await Task.Yield();
            }
            await LoadingScreenUI.Singleton.FadeIn(false);
        }
        #endregion
    }
}