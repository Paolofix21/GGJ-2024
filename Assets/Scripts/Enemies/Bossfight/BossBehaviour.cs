using Code.Player;
using Code.Weapons;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
	}
	
#if UNITY_EDITOR // TEMP to test
	[CustomEditor(typeof(BossBehaviour))]
	public class BossBehaviourCustomEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			
			if (!Application.isPlaying)
				return;
			
			if (GUILayout.Button("Boss Phase 1"))
			{
				((BossBehaviour)target).StartPhase();
			}
		}
	}
#endif
}
