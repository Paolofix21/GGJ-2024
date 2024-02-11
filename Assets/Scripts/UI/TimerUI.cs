using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    #region Public Variables   
    [SerializeField] private TMP_Text m_text;
    #endregion

    #region Properties
    #endregion

    #region Private Variables
    private float timePassed;
    private bool gameOn = true;
    #endregion

    #region Behaviour Callbacks
    private void Update()
    {
        if(gameOn)
        {
            timePassed += Time.deltaTime;
            UpdateTimer(timePassed);
        }
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    private void UpdateTimer(float timer) { 
        if (m_text != null)
        {
            var time = TimeSpan.FromSeconds(timer);
            if (time.Hours < 1)
                m_text.text = time.ToString(@"mm\:ss\.ff");
            else
                m_text.text = time.ToString(@"hh\:mm\:ss\.ff");
        }
    }
    private void SetGameState(bool isOn)
    {
        gameOn = isOn;
    }
    #endregion

    #region Virtual Methods
    #endregion
}
