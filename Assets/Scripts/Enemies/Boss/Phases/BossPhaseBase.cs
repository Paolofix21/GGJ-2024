﻿using System.Threading.Tasks;
using UnityEngine;

namespace Code.EnemySystem.Boss.Phases {
    [System.Serializable]
    public abstract class BossPhaseBase {
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
        protected virtual void OnSetup() {}

        public abstract void Begin();
        public abstract void Execute();
        public abstract void End();

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