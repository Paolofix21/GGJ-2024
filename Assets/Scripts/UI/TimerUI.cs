using Code.Core;
using Code.Core.MatchManagers;
using Code.Data;
using SteamIntegration.Achievements;
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour {
    #region Public Variables
    [SerializeField] private TMP_Text m_text;
    [SerializeField, Min(1f)] private float m_timeForAchievement = 480f;
    [SerializeField] private SteamAchievementSO m_speedRunAchievement;
    #endregion

    #region Properties
    #endregion

    #region Private Variables
    private WaveBasedMatchManager _matchManager;
    private double _timePassed;
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
    private void UpdateTimer(double timer) {
        var time = System.TimeSpan.FromSeconds(timer);
        m_text.text = time.ToString(time.Hours < 1 ? @"mm\:ss\.ff" : @"hh\:mm\:ss\.ff (Noob)");

        GameEvents.GameTime = timer;
    }
    #endregion

    #region Event Methods
    private void OnEndGame(bool didWin) {
        enabled = false;

        if (!didWin)
            return;

        if (_timePassed <= m_timeForAchievement)
            SteamAchievementsController.Singleton?.AdvanceAchievement(m_speedRunAchievement);

        DataManager.GetHighScore(out var highScore);
        Debug.Log(highScore);

        if (highScore < _timePassed)
            return;

        GameEvents.BeatHighScore(_timePassed);

        DataManager.UpdateHighScore(_timePassed);
        DataManager.Apply();
    }
    #endregion
}