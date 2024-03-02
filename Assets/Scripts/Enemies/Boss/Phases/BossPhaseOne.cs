using UnityEngine;

namespace Code.EnemySystem.Boss.Phases {
    [System.Serializable]
    public class BossPhaseOne : BossPhaseBase {
        #region Public Variables
        [SerializeField, Min(0f)] private float m_minAttackDelay = 2f;
        [SerializeField, Min(0.1f)] private float m_maxAttackDelay = 5f;
        [SerializeField, Min(1)] private int m_rounds = 3;
        #endregion

        #region Private Variables
        private BossAttackFireBalls _attackFireBalls;
        #endregion

        #region Overrides
        protected override void OnSetup() => _attackFireBalls = boss.GetComponent<BossAttackFireBalls>();

        protected override void OnBegin() {
            Invoke(TriggerShoot, 1f);
            boss.BossAnimator.OnShoot += Shoot;
        }

        protected override void OnExecute() { }

        protected override void OnEnd() {
            CancelInvoke();
            boss.BossAnimator.OnShoot -= Shoot;
        }
        #endregion

        #region Private Methods
        private void TriggerShoot() {
            var random = Random.Range(m_minAttackDelay, m_maxAttackDelay);
            var duration = boss.BossAnimator.AnimateFireBallsAttack(m_rounds);
            Invoke(TriggerShoot, random + duration);
        }
        #endregion

        #region Event Methods
        private void Shoot() => _attackFireBalls.ShootAt(boss.Target);
        #endregion
    }
}