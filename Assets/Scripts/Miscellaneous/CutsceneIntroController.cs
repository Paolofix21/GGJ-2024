using System.Collections.Generic;
using Code.Core;
using Code.EnemySystem.Wakakas;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Splines;

namespace Miscellaneous {
    [System.Serializable]
    public class CreatureRef {
        public WakakaMaskAnimator creature;
        public string name;
    }

    [RequireComponent(typeof(PlayableDirector))]
    public class CutsceneIntroController : MonoBehaviour {
        #region Public Variables
        [SerializeField] private List<CreatureRef> m_creatures = new();
        [SerializeField] private TextMesh m_text;
        [SerializeField] private SplineAnimate m_splineAnimate;

#if UNITY_EDITOR
        [Range(0, 3)] public int testIndex = 1;
#endif

        public static event System.Action<bool> OnIntroStartStop;
        #endregion

        #region Private Variables
        private PlayableDirector _director;

        private event System.Action _onCutsceneEnded;
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            WaveSystemUI.OnEndWave += OnEndWave;

            gameObject.SetActive(false);
        }

        private void Start() {
            WaveSystemUI.OnEndWave += OnWaveChanged;

            _director = GetComponent<PlayableDirector>();
            _director.timeUpdateMode = DirectorUpdateMode.UnscaledGameTime;
            _director.playOnAwake = false;
            _director.stopped += EndCutscene;
        }

        private void OnEnable() => GameEvents.OnPauseStatusChanged += CheckPause;

        private void Update() {
            if (GameEvents.IsPaused)
                return;

            m_splineAnimate.ElapsedTime += Time.unscaledDeltaTime;
            _director.time = m_splineAnimate.ElapsedTime;
            _director.Evaluate();
        }

        private void OnDisable() => GameEvents.OnPauseStatusChanged -= CheckPause;

        private void OnDestroy() => WaveSystemUI.OnEndWave -= OnWaveChanged;

        private void OnWaveChanged(int waveIndex) {
            GameEvents.SetCutsceneState(true);
            PlayCutscene(waveIndex, () => Time.timeScale = 1);
        }
        #endregion

        #region Public Methods
#if UNITY_EDITOR
        [ContextMenu("Play Cutscene")]
        private void TestPlayback() => PlayCutscene(testIndex, () => Debug.Log("DIO\n"));
#endif

        public void PlayCutscene(int creatureIndex, System.Action onCutsceneEnded) {
            if (_director.state == PlayState.Playing)
                return;

            OnIntroStartStop?.Invoke(true);

            m_splineAnimate.Restart(true);
            for (var i = 0; i < m_creatures.Count; i++)
                m_creatures[i].creature.gameObject.SetActive(i == creatureIndex);

            m_text.text = m_creatures[creatureIndex].name;

            gameObject.SetActive(true);

            _onCutsceneEnded = onCutsceneEnded;
            _director.Play();
        }
        #endregion

        #region Event Methods
        private void EndCutscene(PlayableDirector director) {
            _onCutsceneEnded?.Invoke();
            _onCutsceneEnded = null;
            OnIntroStartStop?.Invoke(false);
            gameObject.SetActive(false);
            GameEvents.SetCutsceneState(false);
        }

        private void OnEndWave(int _) {
            Debug.Log("Dio Fagiano\n");
            OnIntroStartStop?.Invoke(false);
            GameEvents.SetCutsceneState(false);
        }

        private void CheckPause(bool pause) {
            if (pause)
                _director.Pause();
            else
                _director.Resume();
        }
        #endregion
    }
}