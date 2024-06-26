﻿using Code.Weapons;
using UnityEngine;
using Utilities;

namespace Code.EnemySystem.Wakakas {
    public class WakakaHealth : MonoBehaviour, IDamageable {
        #region Public Variables
        [SerializeField, Min(1f)] private float m_maxHealth = 25f;
        [SerializeField] private DamageType m_type;
        [SerializeField] private DamageObject m_immuneTo = DamageObject.None;

        public event System.Action<float> OnHealthChanged;
        public event System.Action<bool> OnEnableDisable;
        public event System.Action OnDeath;
        #endregion

        #region Private Variables
        private float _currentHealth;
        #endregion

        #region Properties
        public DamageObject DamageObject { get; private set; } = DamageObject.Unknown;
        #endregion

        #region Behaviour Callbacks
        private void Awake() => _currentHealth = m_maxHealth;

        private void OnEnable() => OnEnableDisable?.Invoke(true);

        private void Start() => OnHealthChanged?.Invoke(_currentHealth / m_maxHealth);

        private void OnDisable() => OnEnableDisable?.Invoke(false);
        #endregion

        #region IDamageable
        public bool GetDamage(DamageType damageType) => enabled && (m_type & damageType) != 0;

        public void ApplyDamage(float amount, GameObject dealer) {
            DamageObject = DamageObjectHelper.Parse(dealer);
            if (m_immuneTo == DamageObject)
                return;

            _currentHealth -= amount;
            OnHealthChanged?.Invoke(_currentHealth / m_maxHealth);

            if (_currentHealth > 0)
                return;

            OnDeath?.Invoke();
            enabled = false;
        }
        #endregion

        #region Public Methods
        public DamageType GetDamageType() => m_type;

        public float GetCurrent() => _currentHealth / m_maxHealth;
        #endregion
    }
}