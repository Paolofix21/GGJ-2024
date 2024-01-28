using System;
using Code.Dialogue;
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
		private BossPhase[] phases;

		private float remHP;
		private bool isInvulnerable;
		public float HeathAsPercentage => remHP / enemySettings.HP * 100;


#if UNITY_EDITOR
		private void Update()
		{
			// TEST
			if (Input.GetKeyDown(KeyCode.F1))
				StartPhase();
		}
#endif

		private void Start()
		{
			playerPos = GameObject.FindGameObjectWithTag("Player")?.transform;
			playerHealth = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerHealth>();
			phases = GetComponents<BossPhase>();
			remHP = enemySettings.HP;
		}

		public void StartPhase()
		{
			phases[0].StartPhase();
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
			print($"boss now has {remHP} hp");
			if (remHP <= 0)
			{
				Dead();
			}
		}

		// TODO implementare i modifier sul boss
		public void ApplyModifier(DialogueData.BossModifier bossModifier)
		{
			switch (bossModifier)
			{
				case DialogueData.BossModifier.TakeDoubleDamage:
					//throw new NotImplementedException();
					break;
				
				case DialogueData.BossModifier.TakeHalfDamage:
					//throw new NotImplementedException();
					break;
				
				case DialogueData.BossModifier.DealsMoreDamage:
					//throw new NotImplementedException();
					break;
				
				case DialogueData.BossModifier.Heal:
					//throw new NotImplementedException();
					break;
			}
		}
	}
}
