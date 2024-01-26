using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        private float maxHealth = 100f;
        private float currentHealth;

        [SerializeField] private Slider healthBar;

        public event Action<float> OnDamageTaken;
        public event Action<float> OnHeal;
        public event Action OnPlayerDeath;

        private void Start()
        {
            currentHealth = maxHealth;
            healthBar.value = currentHealth / maxHealth;
        }

        private IEnumerator UpdateHealthBar()
        {
            var updateTime = 1f;
            var currentTime = 0f;

            float healthValue = currentHealth / maxHealth;

            while (currentTime < updateTime)
            {
                float t = currentTime / updateTime;
                currentTime += Time.deltaTime;
                healthBar.value = Mathf.Lerp(healthBar.value, healthValue, t*t);
                yield return null;
            }

            healthBar.value = currentHealth / maxHealth;
        }

        public void GetDamage(float _amount)
        {
            currentHealth -= _amount;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            OnDamageTaken?.Invoke(_amount);

            //StopAllCoroutines();
            //StartCoroutine(UpdateHealthBar());

            if (currentHealth <= 0)
                OnPlayerDeath?.Invoke();
        }

        public void Heal(float _amount)
        {
            currentHealth += _amount;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            OnHeal?.Invoke(_amount);

            //StopAllCoroutines();
            //StartCoroutine(UpdateHealthBar());
        }
    }
}

