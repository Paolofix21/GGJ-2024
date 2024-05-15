using Audio;
using UnityEngine;

namespace Code.EnemySystem.Boss.Phases {
    [System.Serializable]
    public class BossPhaseThree : BossPhaseBase<WakakaBossBehaviour> {
        #region Public Variables
        [SerializeField, Min(0.1f)] private float m_delayBetweenVoiceLines = 1.5f;
        [SerializeField] private SoundSO m_preAttackVoiceLine;
        [SerializeField] private SoundSO m_attackVoiceLine;
        #endregion

        #region Private Variables
        private BossAttackTrapezio _attackTrapezio;

        private int _speakingPhase = -1;
        private float _speakTime;
        #endregion

        #region Overrides
        protected override void OnSetup() => _attackTrapezio = boss.GetComponent<BossAttackTrapezio>();

        protected override void OnBegin() {
            Invoke(GoAgain, 1f);
            boss.BossAnimator.OnShoot += Shoot;
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
                    _speakTime = Time.time + boss.BossAnimator.AnimateVoiceLineAuto(m_preAttackVoiceLine.GetSound());
                    break;
                case 3:
                    boss.Health.enabled = true;
                    _speakTime = Time.time + m_delayBetweenVoiceLines;
                    break;
                case 4:
                    boss.Health.enabled = false;
                    _speakTime = Time.time + boss.BossAnimator.AnimateVoiceLineAuto(m_attackVoiceLine.GetSound());
                    break;
                case 5:
                    boss.Health.enabled = true;
                    _speakTime = Time.time + boss.BossAnimator.AnimateTrapezioAttack();
                    break;
                case 6:
                    _speakTime = Time.time + boss.BossAnimator.AnimateDecompose();
                    break;
                case 7:
                    GoAgain();
                    break;
            }
        }

        protected override void OnEnd() {
            CancelInvoke();
            boss.BossAnimator.OnShoot -= Shoot;
            boss.BossAnimator.CancelVoiceLine();
        }
        #endregion

        #region Private Methods
        private void GoAgain() => _speakingPhase = 0;
        #endregion
  
        #region Event Methods
        private void Shoot() => _attackTrapezio.ShootAt(boss.Target);
        #endregion
    }
}