using Audio;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

namespace Code.Player {
    public class PlayerHealth : MonoBehaviour {
        #region Public Variables
        [Header("Sounds")]
        [SerializeField] private SoundSO m_playerDeathSound;
        [SerializeField] private SoundSO m_playerDamageSound;
        [SerializeField] private SoundSO m_playerHealSound;

        [Header("Settings")]
        [FormerlySerializedAs("healthToAdd")]
        [SerializeField] private int m_healthToAdd = 2;
        [FormerlySerializedAs("TimeBeforeStartHealing")]
        [SerializeField] private int m_timeBeforeStartHealing = 3;

        public event System.Action<float, float> OnDamageTaken;
        public event System.Action<float, float> OnHeal;
        public event System.Action OnPlayerDeath;
        #endregion

        #region Private Variables
        private float _maxHealth = 100f;
        private float _currentHealth;
        private int _currentTime;
        #endregion

        #region Properties
        public DamageObject DamageObject { get; private set; } = DamageObject.Unknown;
        #endregion

        #region Behaviour Callbacks
        private void OnEnable() => InvokeRepeating(nameof(CheckHealth), 1, 1);

        private void Start() => _currentHealth = _maxHealth;

        private void OnDisable() => CancelInvoke(nameof(CheckHealth));
        #endregion

        #region Public Methods
        public void DealDamage(float amount, GameObject dealer) {
            if (!enabled)
                return;

            if (_currentHealth <= 0)
                return;

            DamageObject = DamageObjectHelper.Parse(dealer);

            _currentHealth -= amount;
            _currentHealth = Mathf.Clamp(_currentHealth, 0f, _maxHealth);

            OnDamageTaken?.Invoke(_currentHealth, _maxHealth);

            if (_currentHealth <= 0) {
                OnPlayerDeath?.Invoke();
                AudioManager.Singleton.PlayOneShotWorldAttached(m_playerDeathSound.GetSound(), gameObject, MixerType.Voice);
            }
            else {
                AudioManager.Singleton.PlayOneShotWorldAttached(m_playerDamageSound.GetSound(), gameObject, MixerType.Voice);
                _currentTime = m_timeBeforeStartHealing;
            }
        }

        public void Heal(float amount) {
            _currentHealth += amount;
            _currentHealth = Mathf.Clamp(_currentHealth, 0f, _maxHealth);

            OnHeal?.Invoke(_currentHealth, _maxHealth);
        }
        #endregion

        #region Private Methods
        private void CheckHealth() {
            if (_currentTime > 0) {
                _currentTime--;
                if (_currentTime == 0)
                    AudioManager.Singleton.PlayOneShotWorldAttached(m_playerHealSound.GetSound(), gameObject, MixerType.Voice);
                else
                    return;
            }

            if (_currentHealth < _maxHealth) {
                Heal(m_healthToAdd);
            }
        }
        #endregion
    }
}