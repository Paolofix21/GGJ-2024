using Audio;
using Code.EnemySystem.Boss.Phases;
using UnityEngine;

namespace Enemies.BossRoberto.Phases {
    [System.Serializable]
    public class BossRobertoPhaseIdle : BossPhaseBase<WakakaBossRobertoBehaviour> {
        #region Public Variables
        [Header("Settings")]
        [SerializeField] private float m_minWaitTime = 1f;
        [SerializeField] private float m_maxWaitTime = 3f;

        [Space]
        [SerializeField] private SoundSO m_idleVoiceLine;
        #endregion

        #region Private Variables
        private int _speakingPhase = -1;
        private float _speakTime;
        #endregion

        #region Overrides
        protected override void OnBegin() {
            boss.Health.enabled = true;
            _speakingPhase = 0;
        }

        protected override void OnExecute() {
            if (_speakingPhase < 0)
                return;

            if (Time.time < _speakTime)
                return;

            ++_speakingPhase;

            switch (_speakingPhase) {
                case 1:
                    boss.Health.enabled = false;
                    _speakTime = Time.time + boss.BossAnimator.AnimateRecompose();
                    break;
                case 2:
                    _speakTime = Time.time + boss.BossAnimator.AnimateVoiceLineAuto(m_idleVoiceLine.GetSound());
                    break;
                case 3:
                    boss.Health.enabled = true;
                    _speakTime = Time.time + Random.Range(m_minWaitTime, m_maxWaitTime);
                    break;
                case 4:
                    _speakTime = Time.time + boss.BossAnimator.AnimateDecompose();
                    break;
                case 5:
                    boss.SetPhase(WakakaBossRobertoBehaviour.WakakaBossState.PhaseFight);
                    break;
            }
        }

        protected override void OnEnd() { }
        #endregion
    }
}