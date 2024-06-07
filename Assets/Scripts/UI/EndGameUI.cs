using Audio;
using Code.LevelSystem;
using Code.UI;
using Code.Core;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using SteamIntegration.Leaderboard;

[HelpURL("https://www.desmos.com/calculator/l5cxbow5sm")]
public class EndGameUI : MonoBehaviour {
    #region Public Variables
    [Header("Settings")]
    [FormerlySerializedAs("TitleColorVictory")]
    public Color m_titleColorVictory;

    [FormerlySerializedAs("TitleColorGameOver")]
    public Color m_titleColorGameOver;

    [SerializeField] private string m_victoryText = "Victory!", m_loseText = "Game Over!";
    [SerializeField] private string m_highScoreHint = "New High Score: {0}";
    [SerializeField] private string m_retryText = "Try again?";
    [SerializeField] private string m_mainMenuText = "Main Menu";

    [Space]
    [SerializeField] private string m_quitToDesktopText = "Do you really want to return to your Desktop?";

    [SerializeField] private string m_quitToMenuText = "Do you want to return to Main Menu?";

    [Space]
    [SerializeField] private SoundSO m_victorySound;

    [Header("References")]
    [SerializeField] private TMP_Text m_title;

    [SerializeField] private TMP_Text m_titleGlow;
    [SerializeField] private TMP_Text m_hintToDisplay;
    [SerializeField] private TMP_Text m_pointsToDisplay;
    [SerializeField] private TMP_Text m_timeToDisplay;

    [Space]
    [SerializeField] private Button m_simpleButton;
    [SerializeField] private Button m_highlightButton;
    [SerializeField] private Button m_quitButton;

    [FormerlySerializedAs("m_minimumTimeMinutes")]
    [Header("Leaderboard")]
    [SerializeField] private double m_minimumTimeSeconds = 720.0;
    [SerializeField] private double m_scoreCap = 1000.0;
    [SerializeField] private double m_reductionCoefficient = 120.0;
    [SerializeField] private bool m_useScoreCap;
    [Space]
    [SerializeField] private SteamLeaderboardSO m_leaderboard;
    [SerializeField] private SteamLeaderboardSO m_leaderboardTime;
    #endregion

    #region Behaviour Callbacks
    private void Awake() {
        GameEvents.OnEndGame += OnEndGame;
        // GameEvents.OnNewRecordBeaten += OnHighScore;

        m_hintToDisplay.text = string.Empty;
        gameObject.SetActive(false);
    }

    private void Start() => m_quitButton.onClick.AddListener(QuitToDesktop);

    private void OnDestroy() {
        GameEvents.OnEndGame -= OnEndGame;
        // GameEvents.OnNewRecordBeaten -= OnHighScore;
    }
    #endregion

    #region Private Methods
    private void ShowWinningScreen() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        m_title.color = m_titleColorVictory;
        m_titleGlow.color = new Color(m_titleColorVictory.r, m_titleColorVictory.g, m_titleColorVictory.b, 1f);
        m_title.text = m_titleGlow.text = m_victoryText;

        UpdateScore(GameEvents.GameTime);
        m_pointsToDisplay.gameObject.SetActive(true);
        m_timeToDisplay.gameObject.SetActive(true);

        m_simpleButton.gameObject.SetActive(false);

        m_highlightButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = m_mainMenuText;
        m_highlightButton.onClick.AddListener(QuitToMenu);

        AudioManager.Singleton.PlayUiSound(m_victorySound.GetSound());
    }

    private void ShowLosingScreen() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        m_title.color = m_titleColorGameOver;
        m_titleGlow.color = new Color(m_titleColorGameOver.r, m_titleColorGameOver.g, m_titleColorGameOver.b, 1f);
        m_title.text = m_titleGlow.text = m_loseText;

        m_pointsToDisplay.gameObject.SetActive(false);
        m_timeToDisplay.gameObject.SetActive(false);

        m_simpleButton.gameObject.SetActive(true);
        m_simpleButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = m_mainMenuText;
        m_simpleButton.onClick.AddListener(QuitToMenu);

        m_highlightButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = m_retryText;
        m_highlightButton.onClick.AddListener(ReloadCurrentLevel);
    }

    private void Quit() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
    #endregion

    #region Event Methods
    private void QuitToDesktop() => UIManager.Singleton.CallConfirmTask(m_quitToDesktopText, Quit);
    private void QuitToMenu() => UIManager.Singleton.CallConfirmTask(m_quitToMenuText, LoadMainMenu);

    private void OnEndGame(bool didWin) {
        gameObject.SetActive(true);

        if (didWin)
            ShowWinningScreen();
        else
            ShowLosingScreen();
    }

    private void LoadMainMenu() => SceneLoader.LoadScene("MainMenu", LoadSceneMode.Single);

    private void ReloadCurrentLevel() {
        m_highlightButton.interactable = false;
        SceneLoader.ReLoadScenes("Game Scene 01", "Game Scene 01 Waves", "Game Scene 01 UI");
    }

    private void UpdateScore(double timeSeconds) {
        var time = System.TimeSpan.FromSeconds(timeSeconds);
        var points = CalculatePoints(time);
        m_pointsToDisplay.SetText($"Points: {points}");

        var timeString = time.ToString(time.Hours < 1 ? @"mm\:ss\.ff" : @"hh\:mm\:ss\.ff (Noob)");
        m_hintToDisplay.text = string.Format(m_highScoreHint, timeString);
        m_timeToDisplay.SetText($"Time: {timeString}");

        SteamLeaderboardController.Singleton?.SetLeaderboardEntry(m_leaderboard, (int)(points * 1000));
        SteamLeaderboardController.Singleton?.SetLeaderboardEntry(m_leaderboardTime, (int)(timeSeconds * 1000));
    }

    private double CalculatePoints(System.TimeSpan seconds) {
        var totalSeconds = seconds.TotalSeconds;
        var score = m_useScoreCap && totalSeconds < m_minimumTimeSeconds
            ? m_scoreCap
            : (2 * m_scoreCap / System.Math.PI * -System.Math.Atan((totalSeconds - m_minimumTimeSeconds) / m_reductionCoefficient)) + m_scoreCap;

        return score + GameEvents.Score;
    }
    #endregion
}