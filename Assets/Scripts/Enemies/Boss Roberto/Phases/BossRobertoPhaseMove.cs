using System.Collections.Generic;
using Code.EnemySystem.Boss.Phases;
using UnityEngine;

namespace Enemies.BossRoberto.Phases {
    [System.Serializable]
    public class BossRobertoPhaseMove : BossPhaseBase<WakakaBossRobertoBehaviour> {
        #region Public Variables
        [SerializeField] private float m_moveTime = 2f;
        [SerializeField] private AnimationCurve m_moveProgress = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private List<Transform> m_wayPoints = new();
        #endregion

        #region Private Variables
        private float _startTime;
        private Vector3 _startPosition;
        private Transform _targetWayPoint;

        private int _lastWayPointIndex = -1;
        #endregion

        #region Overrides
        protected override void OnBegin() {
            _startTime = Time.time;
            _startPosition = boss.transform.position;

            var wayPoint = Random.Range(0, m_wayPoints.Count);
            if (wayPoint == _lastWayPointIndex)
                wayPoint = ++wayPoint % m_wayPoints.Count;

            _targetWayPoint = m_wayPoints[wayPoint];
            _lastWayPointIndex = wayPoint;
        }

        protected override void OnExecute() {
            var t = (Time.time - _startTime) / m_moveTime;
            var progress = m_moveProgress.Evaluate(t);
            boss.transform.position = Vector3.Slerp(_startPosition, _targetWayPoint.position, progress);
            var rot = Quaternion.Slerp(
                Quaternion.LookRotation(_targetWayPoint.position - boss.transform.position),
                Quaternion.LookRotation(boss.Target.position - boss.transform.position),
                progress);
            boss.transform.rotation = Quaternion.Slerp(boss.transform.rotation, rot, Time.deltaTime * 6f);

            if (t < .99f)
                return;

            boss.SetPhase(WakakaBossRobertoBehaviour.WakakaBossState.PhaseIdle);
        }

        protected override void OnEnd() { }
        #endregion
    }
}