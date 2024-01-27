using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Miscellaneous {
    [System.Serializable]
    public class CreatureRef {
        public GameObject creature;
        public string name;
    }

    [RequireComponent(typeof(PlayableDirector))]
    public class CutsceneIntroController : MonoBehaviour {
        #region Public Variables
        [SerializeField] private List<CreatureRef> m_creatures = new();
        [SerializeField] private TextMesh m_text;
        #endregion

        #region Private Variables
        private PlayableDirector _director;

        private event System.Action _onCutsceneEnded;
        #endregion

        #region Behaviour Callbacks
        private void Awake() {
            _director = GetComponent<PlayableDirector>();
            _director.playOnAwake = false;
            _director.stopped += EndCutscene;

            gameObject.SetActive(false);
        }
        #endregion

        #region Public Methods
        [ContextMenu("Play Cutscene")]
        private void TestPlayback() => PlayCutscene(0, () => Debug.Log("DIO\n"));

        public void PlayCutscene(int creatureIndex, System.Action onCutsceneEnded) {
            if (_director.state == PlayState.Playing)
                return;

            for (var i = 0; i < m_creatures.Count; i++)
                m_creatures[i].creature.SetActive(i == creatureIndex);
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
            gameObject.SetActive(false);
        }
        #endregion
    }
}