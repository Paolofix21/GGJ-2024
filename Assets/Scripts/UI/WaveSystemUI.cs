using Code.EnemySystem;
using Miscellaneous;
using System;
using System.Collections;
using System.Threading.Tasks;
using Code.Core.MatchManagers;
using LanguageSystem.Runtime.Utility;
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

    [Header("Texts")]
    [SerializeField] private LocalizedStringRecord m_waveEndedText;
    [SerializeField] private LocalizedStringRecord m_waveNumberText;
    [SerializeField] private LocalizedStringRecord m_waveLastText;
    #endregion

    #region Events
    public static Action<int> OnEndWave;
    #endregion

    #region Private Variables
    private bool cutsceneState;
    private Coroutine coroutine;
    #endregion

    #region Behaviour Callbacks
    private IEnumerator Start() {
        yield return null;
        WaveBasedMatchManager.Singleton.EntityManager.OnWaveChanged += StartNewWave;
        CutsceneIntroController.OnIntroStartStop += CheckCutscene;
    }

    private void OnDestroy() {
        StopAllCoroutines();
        CutsceneIntroController.OnIntroStartStop -= CheckCutscene;
    }
    #endregion

    #region Public Methods
    public void StartNewWave(int i) {
        if (this != null) // Avoid ReturnMainMenu error
            coroutine = StartCoroutine(NewWaveCO(i));
    }

    private IEnumerator NewWaveCO(int i) {
        if(i != 0) {
            WaveText.text = WaveTextGlow.text = m_waveEndedText.GetLocalizedString();
            WaveObject?.SetActive(true);
            myAnimator.SetTrigger("default");
            yield return new WaitForSeconds(2.0f);
            myAnimator.SetTrigger("go");
            yield return new WaitForSeconds(1.0f);
            WaveObject?.SetActive(false);
        }
        OnEndWave?.Invoke(i);
        string waveText = i != 3 ? string.Format(m_waveNumberText.GetLocalizedString(), i + 1) : m_waveLastText.GetLocalizedString();
        WaveText.text = waveText;
        WaveTextGlow.text = waveText;

        while(cutsceneState)
            yield return Task.Yield();

        yield return new WaitForSeconds(0.5f);
        WaveObject?.SetActive(true);
        myAnimator.SetTrigger("default");
        yield return new WaitForSeconds(5.0f);
        myAnimator.SetTrigger("go");
        yield return new WaitForSeconds(1.0f);
        WaveObject?.SetActive(false);
        coroutine = null;
    }
    public void CheckCutscene(bool isEnded) => cutsceneState = isEnded;
    #endregion
}