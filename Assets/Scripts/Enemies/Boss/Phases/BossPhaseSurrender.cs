using UnityEngine;

namespace Code.EnemySystem.Boss.Phases {
    [System.Serializable]
    public class BossPhaseSurrender : BossPhaseBase<WakakaBossBehaviour> {
        #region Public Variables
        [SerializeField] private float m_pushForce = 5f;
        [SerializeField] private float m_pushRadius = 30f;
        [SerializeField] private Rigidbody[] m_bodies;
        #endregion

        #region Overrides
        protected override void OnBegin() {
            boss.Surrender();

            foreach (var body in m_bodies) {
                body.isKinematic = false;
                body.AddExplosionForce(m_pushForce, boss.transform.position, m_pushRadius);
            }
        }

        protected override void OnExecute() { }

        protected override void OnEnd() { }
        #endregion
    }
}