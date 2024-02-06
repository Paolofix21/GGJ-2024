using UnityEngine;

namespace Code.EnemySystem.Miscellaneous {
    public class ResetTriggerOnExit : StateMachineBehaviour {
        [SerializeField] private string m_triggerName;

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) => animator.ResetTrigger(m_triggerName);
    }
}