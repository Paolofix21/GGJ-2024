using Code.EnemySystem.Boss.Phases;
using UnityEngine;

namespace Enemies.BossRoberto.Phases {
    [System.Serializable]
    public class BossRobertoPhaseSurrender : BossPhaseBase<WakakaBossRobertoBehaviour> {
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

            boss.SetPhase(WakakaBossRobertoBehaviour.WakakaBossState.None);
        }

        protected override void OnExecute() { }

        protected override void OnEnd() {
            Object.Destroy(boss.gameObject, 3f);
        }
        #endregion
    }
}