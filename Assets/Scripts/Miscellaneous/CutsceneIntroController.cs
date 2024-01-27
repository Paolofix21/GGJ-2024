﻿using System.Collections.Generic;
using Code.Graphics;
using UnityEngine;
using UnityEngine.Playables;

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

#if UNITY_EDITOR
        [Range(0, 3)] public int testIndex = 1;
#endif
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
#if UNITY_EDITOR
        [ContextMenu("Play Cutscene")]
        private void TestPlayback() => PlayCutscene(testIndex, () => Debug.Log("DIO\n"));
#endif

        public void PlayCutscene(int creatureIndex, System.Action onCutsceneEnded) {
            if (_director.state == PlayState.Playing)
                return;

            for (var i = 0; i < m_creatures.Count; i++)
                m_creatures[i].creature.gameObject.SetActive(i == creatureIndex);

            m_text.text = m_creatures[creatureIndex].name;
            m_creatures[creatureIndex].creature.SetColorType(creatureIndex);

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