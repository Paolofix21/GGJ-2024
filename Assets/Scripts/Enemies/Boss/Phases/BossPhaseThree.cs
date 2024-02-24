using UnityEngine;

namespace Code.EnemySystem.Boss.Phases {
    [System.Serializable]
    public class BossPhaseThree : BossPhaseBase {
        #region Private Variables
        private BossAttackTrapezio _attackTrapezio;

        private int _speakingPhase = -1;
        private float _speakTime;
        #endregion

        #region Overrides
        protected override void OnSetup() => _attackTrapezio = boss.GetComponent<BossAttackTrapezio>();

        protected override void OnBegin() {
            Invoke(TriggerVoiceLine, 1f);
            boss.BossAnimator.OnShoot += Shoot;
        }

        protected override void OnExecute() {
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

        protected override void OnEnd() {
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