using Code.LevelSystem;
using Code.UI;
using System.Collections;
using System.Collections.Generic;
using Code.EnemySystem.Boss;
using Code.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGameUI : MonoBehaviour
{
    #region Public Variables   
    public Color TitleColorVictory;
    public Color TitleColorGameOver;
    [SerializeField] private TMP_Text m_title;
    [SerializeField] private TMP_Text m_titleGlow;
    [SerializeField] private TMP_Text m_hintToDisplay;
    [SerializeField] private Button m_simpleButton; //on or off
    [SerializeField] private Button m_highlightButton; //try again or main menu
    [SerializeField] private Button m_quitButton;
    #endregion

    #region Properties
    #endregion

    #region Private Variables

    public enum EndgameState { Victory, GameOver};
    private EndgameState _endgameState;
    #endregion

    #region Behaviour Callbacks
    private void Awake()
    {
        m_simpleButton.onClick.RemoveAllListeners();
        m_highlightButton.onClick.RemoveAllListeners();
        m_quitButton.onClick.RemoveAllListeners();
    }
    private void Start()
    {
        m_quitButton.onClick.AddListener(delegate { UIManager.Singleton.CallConfirmTask("Do you really want to return to your desktop?", Application.Quit); });
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    public void CallEndgame(EndgameState state)
    {
        Cursor.lockState = CursorLockMode.None;
        string textEndgame = null;
        string hint = null;
        Color32 newColor = Color.white;
        switch (state)
        {
            case EndgameState.Victory:
                newColor = TitleColorVictory;
                textEndgame = "Victory!";
                hint = "";
                m_simpleButton.gameObject.SetActive(false);
                m_highlightButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "main menu";
                m_highlightButton.onClick.AddListener(delegate { UIManager.Singleton.CallConfirmTask("Do you want to return to Main Menu?", LoadScene); });
                break;
            case EndgameState.GameOver:
                newColor = TitleColorGameOver;
                textEndgame = "Game Over!";
                hint = "";
                m_simpleButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "main menu";
                m_simpleButton.onClick.AddListener(delegate { UIManager.Singleton.CallConfirmTask("Do you want to return to Main Menu?", LoadScene); });
                m_highlightButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = "try again";
                m_highlightButton.onClick.AddListener(ResetScene);
                break;
        }
        m_title.color = new Color(newColor.r, newColor.g, newColor.b, 255);
        m_title.text = textEndgame;
        m_titleGlow.text = textEndgame;
        m_hintToDisplay.text = hint;
    }
    private void LoadScene()
    {
        SceneLoader.LoadScene("MainMenu", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
    private void ResetScene()
    {
        SceneLoader.LoadScene("Hell", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
    #endregion

    #region Virtual Methods
    #endregion
}
