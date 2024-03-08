using System;
using Audio;
using UnityEngine;

namespace Code.Player {
    public class PlayerHealth : MonoBehaviour {
        private float maxHealth = 100f;
        private float currentHealth;

        [SerializeField] private SoundSO m_playerDeathSound;
        [SerializeField] private SoundSO m_playerDamageSound;
        [SerializeField] private SoundSO m_playerHealSound;

        [Space]
        [SerializeField] private int healthToAdd;
        [SerializeField] private int TimeBeforeStartHealing;
        private int currentTime;

        public event Action<float, float> OnDamageTaken;
        public event Action<float, float> OnHeal;
        public event Action OnPlayerDeath;

        private void OnEnable() => InvokeRepeating(nameof(CheckHealth), 1, 1);

        private void OnDisable() => CancelInvoke(nameof(CheckHealth));

        private void Start() => currentHealth = maxHealth;

        public void GetDamage(float _amount) {
            if (!enabled)
                return;

            if (currentHealth <= 0)
                return;

            currentHealth -= _amount;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            OnDamageTaken?.Invoke(currentHealth, maxHealth);

            if (currentHealth <= 0) {
                OnPlayerDeath?.Invoke();
                AudioManager.Singleton.PlayOneShotWorldAttached(m_playerDeathSound.GetSound(), gameObject, MixerType.Voice);
            }
            else {
                AudioManager.Singleton.PlayOneShotWorldAttached(m_playerDamageSound.GetSound(), gameObject, MixerType.Voice);
                currentTime = TimeBeforeStartHealing;
            }
        }

        private void CheckHealth() {
            if (currentTime > 0) {
                currentTime--;
                if (currentTime == 0)
                    AudioManager.Singleton.PlayOneShotWorldAttached(m_playerHealSound.GetSound(), gameObject, MixerType.Voice);
                else
                    return;
            }

            if (currentHealth < maxHealth) {
                Heal(healthToAdd);
            }
        }

        public void Heal(float _amount) {
            currentHealth += _amount;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            OnHeal?.Invoke(currentHealth, maxHealth);
        }
    }
}