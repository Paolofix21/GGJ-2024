﻿using Code.Player;
using UnityEngine;

namespace Code.EnemySystem.Wakakas {
    public class WakakaAttacker : MonoBehaviour {
        #region Public Variables
        [SerializeField, Min(0.1f)] private float m_dot = 5f;
        [SerializeField, Min(0.1f)] private float m_damageRate = .75f;
        [SerializeField] private WakakaMaskAnimator m_maskAnimator;
        #endregion

        #region Private Variables
        private PlayerHealth _target;
        #endregion

        #region Behaviour Callbacks
        private void OnTriggerEnter(Collider other) {
            if (!other.CompareTag("Player"))
                return;

            if (other.TryGetComponent(out _target))
                InvokeRepeating(nameof(DamageTarget), 0f,m_damageRate);
        }

        private void OnTriggerExit(Collider other) {
            if (!other.CompareTag("Player"))
                return;

            CancelInvoke();
            _target = null;
        }

        private void OnDestroy() {
            CancelInvoke();
            _target = null;
        }
        #endregion

        #region Private Methods
        private void DamageTarget() {
            m_maskAnimator.AnimateLaughter();
            _target?.DealDamage(m_dot, gameObject);
        }
        #endregion
    }
}