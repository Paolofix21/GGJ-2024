using Audio;
using UnityEngine;

namespace Code.EnemySystem.Boss.Phases {
    [System.Serializable]
    public class BossPhaseOne : BossPhaseBase<WakakaBossBehaviour> {
        #region Public Variables
        [SerializeField, Min(0f)] private float m_minAttackDelay = 2f;
        [SerializeField, Min(0.1f)] private float m_maxAttackDelay = 5f;
        [SerializeField, Min(1)] private int m_rounds = 3;

        [Space]
        [SerializeField] private SoundSO m_attackVoiceLine;
        #endregion

        #region Private Variables
        private BossAttackFireBalls _attackFireBalls;

        private int _speakingPhase = -1;
        private float _speakTime;
        #endregion

        #region Overrides
        protected override void OnSetup() => _attackFireBalls = boss.GetComponent<BossAttackFireBalls>();

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
                    _speakTime = Time.time + boss.BossAnimator.AnimateVoiceLineAuto(m_attackVoiceLine.GetSound());
                    break;
                case 3:
                    boss.Health.enabled = true;
                    _speakTime = Time.time + boss.BossAnimator.AnimateDecompose();
                    break;
                case 4:
                    _speakTime = Time.time + boss.BossAnimator.AnimateFireBallsAttack(m_rounds);
                    break;
                case 5:
                    _speakTime = Time.time + Random.Range(m_minAttackDelay, m_maxAttackDelay);
                    break;
                case 6:
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
        private void Shoot() => _attackFireBalls.ShootAt(boss.Target);
        #endregion
    }
}