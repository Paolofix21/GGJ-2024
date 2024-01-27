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

        private void Start()
        {
            currentHealth = maxHealth;
            healthBar.value = currentHealth / maxHealth;
        }

        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.Q))
            //    GetDamage(30);
            //if (Input.GetKeyDown(KeyCode.E))
            //    Heal(30);
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

            StopAllCoroutines();
            StartCoroutine(UpdateHealthBar());

            if (currentHealth <= 0)
                Debug.Log("Game Over");
        }

        private void Heal(float _amount)
        {
            currentHealth += _amount;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
            StopAllCoroutines();
            StartCoroutine(UpdateHealthBar());
        }
    }
}

