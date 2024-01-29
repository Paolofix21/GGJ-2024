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

        [SerializeField] private int healthToAdd;
        [SerializeField] private int TimeBeforeStartHealing;
        private int currentTime;

        public event Action<float, float> OnDamageTaken;
        public event Action<float, float> OnHeal;
        public event Action OnPlayerDeath;

        private void OnEnable()
        {
            InvokeRepeating(nameof(CheckHealth), 1, 1);
        }

        private void OnDisable()
        {
            CancelInvoke(nameof(CheckHealth));
        }

        private void Start()
        {
            currentHealth = maxHealth;
        }

        public void GetDamage(float _amount)
        {
            currentHealth -= _amount;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            OnDamageTaken?.Invoke(currentHealth, maxHealth);

            if (currentHealth <= 0)
            {
                OnPlayerDeath?.Invoke();
                AudioManager.instance.PlayOneShot(FMODEvents.instance.playerDeathEvent, this.transform.position);
            }
            else
            {
                AudioManager.instance.PlayOneShot(FMODEvents.instance.playerTakeDamageEvent, this.transform.position);
                currentTime = TimeBeforeStartHealing;
            }
        }

        private void CheckHealth() 
        { 
            if(currentTime > 0)
            {
                currentTime--;
                if (currentTime == 0)
                    AudioManager.instance.PlayOneShot(FMODEvents.instance.playerHealEvent, this.transform.position);
                else
                    return;
            }

            if(currentHealth < maxHealth)
            {
                Heal(healthToAdd);
            }
        }

        public void Heal(float _amount)
        {
            currentHealth += _amount;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            OnHeal?.Invoke(currentHealth, maxHealth);
        }
    }
}

