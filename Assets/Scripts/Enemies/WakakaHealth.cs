﻿using Code.Weapons;
using UnityEngine;

namespace Code.EnemySystem {
    public class WakakaHealth : MonoBehaviour, IDamageable {
        #region Public Variables
        [SerializeField, Min(1f)] private float m_maxHealth = 25f;
        [SerializeField] private DamageType m_type;

        public event System.Action<float> OnHealthChanged;
        public event System.Action OnDeath;
        #endregion

        #region Private Variables
        private float _currentHealth;
        #endregion

        #region Properties
        #endregion

        #region Behaviour Callbacks
        private void Awake() => _currentHealth = m_maxHealth;

        private void Start() => OnHealthChanged?.Invoke(_currentHealth / m_maxHealth);
        #endregion

        #region IDamageable
        public bool GetDamage(DamageType damageType) => damageType.HasFlag(m_type);

        public void ApplyDamage(float amount) {
            _currentHealth -= amount;
            OnHealthChanged?.Invoke(_currentHealth / m_maxHealth);

            if (_currentHealth > 0)
                return;

            OnDeath?.Invoke();
        }
        #endregion

        #region Public Methods
        public DamageType GetDamageType() => m_type;
        #endregion

        #region Private Methods
        #endregion

        #region Event Methods
        #endregion
    }
}