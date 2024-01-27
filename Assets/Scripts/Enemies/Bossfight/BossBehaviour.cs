using Code.Player;
using Code.Weapons;
using UnityEngine;

namespace Code.EnemySystem.Boss
{
	/// <summary>
	/// Handles the behaviour for the wave boss.
	/// </summary>
	public class BossBehaviour : MonoBehaviour, IDamageable
	{
		public EnemySettings enemySettings;

		private Transform playerPos;
		private PlayerHealth playerHealth;
		private Vector3 wanderDirection;

		private float remHP;
		private bool isInvulnerable;
		public float HeathAsPercentage => remHP / enemySettings.HP * 100;


		private void Start()
		{
			playerPos = GameObject.FindGameObjectWithTag("Player").transform;
			playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
			remHP = enemySettings.HP;
		}
		
		private void Dead()
		{
			Destroy(gameObject);
		}
		
		public bool GetDamage(DamageType damageType)
		{
			return !isInvulnerable && damageType.HasFlag(enemySettings.DamageType);
		}

		public void ApplyDamage(float amount)
		{
			remHP -= amount;

			if (remHP <= 0)
			{
				Dead();
			}
		}
	}
}
