﻿using UnityEngine;

namespace Code.EnemySystem.Boss.Phases {
    [System.Serializable]
    public class BossPhaseThree : BossPhaseBase {
        #region Public Variables
        [SerializeField, Min(0f)] private float m_minAttackDelay = 2f;
        [SerializeField, Min(0.1f)] private float m_maxAttackDelay = 5f;
        [SerializeField, Min(1)] private int m_rounds = 3;
        #endregion

        #region Private Variables
        private BossAttackTrapezio _attackTrapezio;

        private int _speakingPhase = -1;
        private float _speakTime;
        #endregion

        #region Overrides
        protected override void OnSetup() => _attackTrapezio = boss.GetComponent<BossAttackTrapezio>();

        public override void Begin() {
            Invoke(TriggerVoiceLine, 1f);
            boss.BossAnimator.OnShoot += Shoot;
        }

        public override void Execute() {
            if (_speakingPhase < 0)
                return;

            if (Time.time < _speakTime)
                return;

            ++_speakingPhase;

            switch (_speakingPhase) {
                case 1:
                    _speakTime = Time.time + boss.BossAnimator.AnimateVoiceLineAuto();
                    break;
                case 2:
                    _speakTime = Time.time + boss.BossAnimator.AnimateTrapezioAttack();
                    break;
                case 3:
                    _speakTime = Time.time + boss.BossAnimator.AnimateDecompose();
                    break;
                case 4:
                    TriggerVoiceLine();
                    break;
            }
        }

        public override void End() {
            CancelInvoke();
            boss.BossAnimator.OnShoot -= Shoot;
        }
        #endregion

        #region Private Methods
        private void TriggerVoiceLine() {
            _speakTime = Time.time + boss.BossAnimator.AnimateRecompose();
            _speakingPhase = 0;
        }
        #endregion
  
        #region Event Methods
        private void Shoot() => _attackTrapezio.ShootAt(boss.Target);
        #endregion
    }
}