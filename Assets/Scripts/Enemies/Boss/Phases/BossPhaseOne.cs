﻿using UnityEngine;

namespace Code.EnemySystem.Boss.Phases {
    [System.Serializable]
    public class BossPhaseOne : BossPhaseBase {
        #region Public Variables
        [SerializeField, Min(0f)] private float m_minAttackDelay = 2f;
        [SerializeField, Min(0.1f)] private float m_maxAttackDelay = 5f;
        [SerializeField, Min(1)] private int m_rounds = 3;
        #endregion

        #region Overrides
        public override void Begin() => Invoke(TriggerShoot, 1f);

        public override void Execute() { }

        public override void End() => CancelInvoke();
        #endregion

        #region Private Methods
        private void TriggerShoot() {
            var random = Random.Range(m_minAttackDelay, m_maxAttackDelay);
            var duration = boss.BossAnimator.AnimateFireBallsAttack(m_rounds);
            Invoke(TriggerShoot, random + duration);
        }
        #endregion
    }
}