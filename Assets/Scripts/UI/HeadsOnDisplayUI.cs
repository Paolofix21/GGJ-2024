using System.Collections.Generic;
using Code.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.UI {
    public sealed class HeadsOnDisplayUI : UIBehaviour {
        #region Public Variables
        [SerializeField] private List<GameObject> m_hideOnCutsceneOnly = new();
        [SerializeField] private List<GameObject> m_hideAlways = new();
        #endregion

        #region Behaviour Callbacks
        protected override void Awake() {
            GameEvents.OnCutsceneStateChanged += Check;
            GameEvents.OnPauseStatusChanged += Check;
        }

        protected override void OnDestroy() {
            GameEvents.OnCutsceneStateChanged -= Check;
            GameEvents.OnPauseStatusChanged -= Check;
        }
        #endregion

        #region Event Methods
        private void Check(bool _) {
            m_hideAlways.ForEach(go => go.SetActive(!GameEvents.IsOnHold));
            m_hideOnCutsceneOnly.ForEach(go => go.SetActive(!GameEvents.IsCutscenePlaying));
        }
        #endregion
    }
}