using Code.EnemySystem;
using System.Collections;
using TMPro;
using UnityEngine;

public class WaveSystemUI : MonoBehaviour
{
    #region Public Variables   
    [SerializeField] private GameObject WaveObject;
    [SerializeField] private TMP_Text WaveText;
    [SerializeField] private TMP_Text WaveTextGlow;
    [SerializeField] private Animator myAnimator;
    #endregion

    #region Behaviour Callbacks
    private void Awake() => WaveSpawner.OnMacroWaveIndexChanged += StartNewWave;

    private void OnDestroy() => WaveSpawner.OnMacroWaveIndexChanged -= StartNewWave;
    #endregion

    #region Public Methods
    public void StartNewWave(int i) {
        StartCoroutine(NewWaveCO($"Wave {i+1}"));
    }

    private IEnumerator NewWaveCO(string wave) {
        WaveTextGlow.text = wave;
        WaveText.text = wave;
        yield return new WaitForSeconds(1);
        WaveObject.SetActive(true);
        myAnimator.SetTrigger("default");
        yield return new WaitForSeconds(5);
        myAnimator.SetTrigger("go");
        yield return new WaitForSeconds(1);
        WaveObject.SetActive(false);
    }
    #endregion
}
