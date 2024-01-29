using Code.EnemySystem;
using Miscellaneous;
using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class WaveSystemUI : MonoBehaviour
{
    #region Public Variables   
    [SerializeField] private GameObject WaveObject;
    [SerializeField] private TMP_Text WaveText;
    [SerializeField] private TMP_Text WaveTextGlow;
    [SerializeField] private Animator myAnimator;
    #endregion

    #region Events
    public static Action<int> OnEndWave;
    #endregion

    #region Private Variables
    private bool cutsceneState;
    #endregion

    #region Behaviour Callbacks
    private void Awake()
    {
        WaveSpawner.OnMacroWaveIndexChanged += NewWave;
        CutsceneIntroController.OnIntroStartStop += CheckCutscene;
    }

    private void OnDestroy()
    {
        WaveSpawner.OnMacroWaveIndexChanged -= NewWave;
        CutsceneIntroController.OnIntroStartStop -= CheckCutscene;
    }
    #endregion

    #region Public Methods

    private async void NewWave(int i) {
        if(i != 0) {
            WaveText.text = "Wave ended";
            WaveTextGlow.text = "Wave ended";
            WaveObject?.SetActive(true);
            myAnimator.SetTrigger("default");
            await Task.Delay(2000);
            myAnimator.SetTrigger("go");
            await Task.Delay(1000);
            WaveObject?.SetActive(false);
        }
        OnEndWave?.Invoke(i);
        string waveText = $"Wave {i + 1}";
        WaveText.text = waveText;
        WaveTextGlow.text = waveText;

        while(cutsceneState)
            await Task.Yield();

        await Task.Delay(500);
        WaveObject?.SetActive(true);
        myAnimator.SetTrigger("default");
        await Task.Delay(5000);
        myAnimator.SetTrigger("go");
        await Task.Delay(1000);
        WaveObject?.SetActive(false);
    }
    public void CheckCutscene(bool isEnded)
    {
        cutsceneState = isEnded;
    }
    #endregion
}
