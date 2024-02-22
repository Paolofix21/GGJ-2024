using System;
using Code.Core;
using Code.Core.MatchManagers;
using Code.Data;
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour {
    #region Public Variables
    [SerializeField] private TMP_Text m_text;
    #endregion

    #region Properties
    #endregion

    #region Private Variables
    private WaveBasedMatchManager _matchManager;
    private float _timePassed;
    #endregion

    #region Behaviour Callbacks
    private void Awake() {
        if (m_text != null) {
            GameEvents.OnEndGame += OnEndGame;
            return;
        }

        Debug.LogWarning("The variable text was not assigned...\n", this);
        enabled = false;
    }

    private void Start() => _matchManager = GameEvents.GetMatchManager<WaveBasedMatchManager>();

    private void Update() {
        if (GameEvents.IsOnHold)
            return;

        if (!_matchManager.IsOngoing)
            return;

        _timePassed += Time.deltaTime;
        UpdateTimer(_timePassed);
    }

    private void OnDestroy() => GameEvents.OnEndGame -= OnEndGame;
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    private void UpdateTimer(float timer) {
        var time = TimeSpan.FromSeconds(timer);
        m_text.text = time.ToString(time.Hours < 1 ? @"mm\:ss\.ff" : @"hh\:mm\:ss\.ff (Noob)");
    }
    #endregion

    #region Event Methods
    private void OnEndGame(bool didWin) {
        enabled = false;
        DataManager.GetHighScore(out var highScore);

        var span = TimeSpan.FromSeconds(_timePassed);
        if (highScore < span)
            return;

        GameEvents.BeatHighScore(_timePassed);

        DataManager.UpdateHighScore(span);
        DataManager.Apply();
    }
    #endregion
}