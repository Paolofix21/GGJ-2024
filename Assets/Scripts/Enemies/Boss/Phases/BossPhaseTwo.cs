using UnityEngine;

namespace Code.EnemySystem.Boss.Phases {
    [System.Serializable]
    public class BossPhaseTwo : BossPhaseBase {
        #region Public Variables
        [SerializeField, Min(0f)] private float m_minAttackDelay = 2f;
        [SerializeField, Min(0.1f)] private float m_maxAttackDelay = 5f;
        [SerializeField, Min(1f)] private float m_duration = 4f;
        #endregion

        #region Properties
        public float Duration => m_duration;
        #endregion

        #region Overrides
        public override void Begin() => Invoke(TriggerShoot, 1f);

        public override void Execute() { }

        public override void End() => CancelInvoke();
        #endregion

        #region Private Methods
        private void TriggerShoot() {
            var random = Random.Range(m_minAttackDelay, m_maxAttackDelay);
            var duration = boss.BossAnimator.AnimateLaserBeamAttack(m_duration);
            Invoke(TriggerShoot, random + duration);
        }
        #endregion
    }
}