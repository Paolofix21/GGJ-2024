﻿using UnityEngine;

namespace Code.EnemySystem.Boss {
    public class AnimationEventTriggerOnEnter : StateMachineBehaviour {
        [SerializeField] private string m_eventName;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) => animator.SendMessage(m_eventName);
    }
}