using FMODUnity;
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

        //in ordine: HP rimossi, HP attuali, HP max
        public event Action<float, float, float> OnDamageTaken;
        public event Action<float, float, float> OnHeal;
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
            Debug.Log("AHIAHIA");
            currentHealth -= _amount;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            OnDamageTaken?.Invoke(_amount, currentHealth, maxHealth);

            

            if (currentHealth <= 0)
            {
                OnPlayerDeath?.Invoke();
                AudioManager.instance.PlayOneShot(FMODEvents.instance.playerDeathEvent, this.transform.position);
            }
            else
            {
                AudioManager.instance.PlayOneShot(FMODEvents.instance.playerTakeDamageEvent, this.transform.position);
            }
        }

        public void Heal(float _amount)
        {
            currentHealth += _amount;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            AudioManager.instance.PlayOneShot(FMODEvents.instance.playerHealEvent, this.transform.position);

            OnHeal?.Invoke(_amount, currentHealth, maxHealth);
        }
    }
}

