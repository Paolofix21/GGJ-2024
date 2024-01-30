using UnityEngine;

namespace Code.EnemySystem.Boss.Phases {
    [System.Serializable]
    public class BossPhaseOne : BossPhaseBase {
        #region Public Variables
        [SerializeField] private float m_minAttackDelay = 2f;
        [SerializeField] private float m_maxAttackDelay = 5f;
        #endregion

        #region Private Variables
        #endregion

        #region Properties
        #endregion

        #region Constructors
        #endregion

        #region Overrides
        public override void Begin() => Invoke(TriggerShoot, 1f);

        public override void Execute() { }

        public override void End() => CancelInvoke();
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        private void TriggerShoot() {
            var random = Random.Range(m_minAttackDelay, m_maxAttackDelay);
            boss.Animator.AnimateAttack(0, random);
            Invoke(TriggerShoot, random + 3f);
        }
        #endregion

        #region Event Methods
        #endregion
    }
}