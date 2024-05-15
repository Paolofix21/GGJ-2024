using Audio;
using Code.EnemySystem.Boss.Phases;
using UnityEngine;

namespace Enemies.BossRoberto.Phases {
    [System.Serializable]
    public class BossRobertoPhaseFight : BossPhaseBase<WakakaBossRobertoBehaviour> {
        #region Public Variables
        [SerializeField, Min(1)] private int m_minNumberOfShots = 3;
        [SerializeField, Min(2)] private int m_maxNumberOfShots = 5;
        [SerializeField, Min(0.01f)] private float m_delayBetweenShots = 0.35f;

        [Space]
        [SerializeField] private SoundSO m_attackVoiceLine;
        #endregion

        #region Properties
        private BossRobertoAttackCameras _attackCameras;

        private int _numOfShots;
        private int _speakingPhase = -1;
        private float _speakTime;
        #endregion

        #region Overrides
        protected override void OnSetup() => _attackCameras = boss.GetComponent<BossRobertoAttackCameras>();

        protected override void OnBegin() => Reset();

        protected override void OnExecute() {
            if (_speakingPhase < 0)
                return;

            if (Time.time < _speakTime)
                return;

            ++_speakingPhase;

            switch (_speakingPhase) {
                case 1:
                    // boss.Health.enabled = false;
                    // _speakTime = Time.time + boss.BossAnimator.AnimateRecompose();
                    break;
                case 2:
                    // _speakTime = Time.time + boss.BossAnimator.AnimateVoiceLineAuto(m_attackVoiceLine.GetSound());
                    break;
                case 3:
                    boss.Health.enabled = true;
                    _numOfShots = Random.Range(m_minNumberOfShots, m_maxNumberOfShots + 1);
                    Debug.Log(_numOfShots);
                    _speakTime = Time.time + boss.BossAnimator.AnimateCamerasAttack(_numOfShots * m_delayBetweenShots + 0.15f);
                    _attackCameras.ActivateCameras();
                    Invoke(Shoot, 0.1f);
                    break;
                case 4:
                    // _speakTime = Time.time + boss.BossAnimator.AnimateDecompose();
                    _attackCameras.DeactivateCameras();
                    break;
                case 5:
                    boss.SetPhase(WakakaBossRobertoBehaviour.WakakaBossState.PhaseMove);
                    break;
                default:
                    return;
            }
        }

        protected override void OnEnd() {
            CancelInvoke();
            boss.BossAnimator.CancelVoiceLine();
        }
        #endregion

        #region Private Methods
        private void Reset() => _speakingPhase = 0;
        #endregion

        #region Event Methods
        private void Shoot() {
            --_numOfShots;
            _attackCameras.ShootAt(boss.Target);
            Debug.Log(_numOfShots);

            if (_numOfShots <= 0)
                return;

            Invoke(Shoot, m_delayBetweenShots);
        }
        #endregion
    }
}