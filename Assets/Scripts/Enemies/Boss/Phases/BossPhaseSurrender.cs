﻿using UnityEngine;

namespace Code.EnemySystem.Boss.Phases {
    [System.Serializable]
    public class BossPhaseSurrender : BossPhaseBase {
        #region Public Variables
        [SerializeField] private float m_pushForce = 5f;
        [SerializeField] private float m_pushRadius = 30f;
        [SerializeField] private Rigidbody[] m_bodies;
        #endregion

        #region Overrides
        public override void Begin() {
            boss.Surrender();

            foreach (var body in m_bodies) {
                // body.transform.SetParent(null, true);
                body.isKinematic = false;
                body.AddExplosionForce(m_pushForce, boss.transform.position, m_pushRadius);
            }
        }

        public override void Execute() { }

        public override void End() { }
        #endregion
    }
}