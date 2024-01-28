using Code.EnemySystem;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    #region Properties
    #endregion

    #region Private Variables
    #endregion

    #region Behaviour Callbacks
    private void Start()
    {
        WaveSpawner.OnMacroWaveIndexChanged += StartNewWave;
    }
    #endregion

    #region Public Methods
    public async void StartNewWave(int i)
    {
        WaveText.text = $"Wave {i+1}";
        WaveTextGlow.text = $"Wave {i+1}";
        WaveObject.SetActive(true);
        await Task.Delay(5000);
        myAnimator.SetTrigger("go");
        await Task.Delay(1000);
        WaveObject.SetActive(false);
    }
    #endregion

    #region Private Methods
    #endregion

    #region Virtual Methods
    #endregion
}
