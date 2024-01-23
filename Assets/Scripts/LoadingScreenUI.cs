using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class LoadingScreenUI : MonoBehaviour
{
    #region Public Variables
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private Animator m_animator;
    //[SerializeField] private Slider m_slider;
    #endregion

    #region Properties
    public static LoadingScreenUI Singleton { get; private set; }
    #endregion

    #region Private Variables
    private int currentCount;
    #endregion

    #region Behaviour Callbacks
    private void Awake()
    {
#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
            return;
#endif
        if (Singleton && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        Singleton = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
        m_canvasGroup.alpha = 0.0f;
    }
    private void OnDestroy()
    {
        if (Singleton != this)
            return;

        Singleton = null;
    }
    #endregion

    #region Public Methods
    public async Task FadeIn(bool isOn)
    {
        currentCount = isOn ? currentCount++ : currentCount--;
        if (currentCount > 1 || (currentCount == 1 && !isOn))
        {
            return;
        }
        m_animator.SetBool("go", isOn);
        while (m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            await Task.Yield();
        }
    }
    #endregion

    #region Private Methods
    #endregion

    #region Virtual Methods
    #endregion
}
