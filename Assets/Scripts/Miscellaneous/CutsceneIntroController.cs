using System.Collections.Generic;
using Code.EnemySystem;
using Code.Graphics;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Splines;
using Code.UI;

namespace Miscellaneous {
    [System.Serializable]
    public class CreatureRef {
        public MaskAnimator creature;
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
        #endregion

        #region Private Variables
        private PlayableDirector _director;

        private event System.Action _onCutsceneEnded;
        #endregion

        public static event System.Action<bool> OnIntroStartStop;

        #region Behaviour Callbacks
        private void Awake() {
            WaveSystemUI.OnEndWave += OnWaveChanged;

            _director = GetComponent<PlayableDirector>();
            _director.timeUpdateMode = DirectorUpdateMode.UnscaledGameTime;
            _director.playOnAwake = false;
            _director.stopped += EndCutscene;

            gameObject.SetActive(false);
        }

        private void Update() => m_splineAnimate.ElapsedTime += Time.unscaledDeltaTime;

        private void OnDestroy() {
            WaveSystemUI.OnEndWave -= OnWaveChanged;
        }

        private void OnWaveChanged(int waveIndex) {
            Time.timeScale = 0;
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
            // m_creatures[creatureIndex].creature.SetColorType(creatureIndex);

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
        }
        #endregion
    }
}