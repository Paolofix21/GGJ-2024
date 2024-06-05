using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Code.UI;

namespace Code.LevelSystem {
    public static class SceneLoader {
        #region Public Methods
        public static async void LoadScene(string sceneName, LoadSceneMode mode) {
            await LoadingScreenUI.Singleton.FadeIn(true);
            await SceneManager.LoadSceneAsync(sceneName, mode);
            await LoadingScreenUI.Singleton.FadeIn(false);
        }

        public static async void LoadScenes(string sceneName, params string[] additives) {
            await LoadingScreenUI.Singleton.FadeIn(true);

            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            foreach (var additive in additives)
                await SceneManager.LoadSceneAsync(additive, LoadSceneMode.Additive);

            await LoadingScreenUI.Singleton.FadeIn(false);
        }

        public static async void ReLoadScenes(string sceneName, params string[] additives) {
            await LoadingScreenUI.Singleton.FadeIn(true);

            await SceneManager.LoadSceneAsync("LoadUtilityScene", LoadSceneMode.Single);
            await Task.Yield();

            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            foreach (var additive in additives)
                await SceneManager.LoadSceneAsync(additive, LoadSceneMode.Additive);

            await LoadingScreenUI.Singleton.FadeIn(false);
        }
        #endregion
    }
}