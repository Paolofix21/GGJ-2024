using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader{
    #region Public Variables   
    #endregion

    #region Properties
    #endregion

    #region Private Variables
    #endregion

    #region Behaviour Callbacks
    #endregion

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

    #region Private Methods
    #endregion

    #region Virtual Methods
    #endregion
}
