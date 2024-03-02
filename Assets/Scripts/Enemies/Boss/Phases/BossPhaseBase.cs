using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Code.EnemySystem.Boss.Phases {
    [System.Serializable]
    public abstract class BossPhaseBase {
        #region Public Variables
        [FormerlySerializedAs("onBeginEndPhase")] public UnityEvent<bool> onBeginOrEndPhase;
        #endregion

        #region Private Variables
        protected WakakaBossBehaviour boss;

        private bool _interrupt;
        #endregion

        #region Constructors
        public void SetUp(WakakaBossBehaviour bossBehaviour) {
            boss = bossBehaviour;
            OnSetup();
        }
        #endregion

        #region Public Methods
        public void Begin() {
            Debug.Log($"Begin '{GetType().Name}' phase...\n");
            onBeginOrEndPhase?.Invoke(true);
            OnBegin();
        }

        public void Execute() => OnExecute();

        public void End() {
            Debug.Log($"...end '{GetType().Name}' phase\n");
            OnEnd();
            onBeginOrEndPhase?.Invoke(false);
        }
        #endregion

        #region Virtual Methods
        protected virtual void OnSetup() {}

        protected abstract void OnBegin();
        protected abstract void OnExecute();
        protected abstract void OnEnd();

        public virtual void OnGUI() => GUILayout.Label(GetType().Name);
        #endregion

        #region Private Methods
        protected void CancelInvoke() => _interrupt = true;

        protected async void Invoke(System.Action method, float secondsDelay) {
            var t = 0f;

            while (t < secondsDelay) {
                if (_interrupt) {
                    _interrupt = false;
                    return;
                }

                t += Time.deltaTime;
                await Task.Yield();
            }

            method?.Invoke();
        }
        #endregion
    }
}