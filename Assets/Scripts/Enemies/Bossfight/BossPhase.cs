using System;
using System.Collections;
using System.Threading;
using Code.Dialogue;
using UnityEngine;

namespace Code.EnemySystem.Boss
{
	[RequireComponent(typeof(BossBehaviour))]
	public abstract class BossPhase : MonoBehaviour
	{
		public static event Action OnPhaseEnded;
		protected BossBehaviour boss;
		protected WaveSpawner waveSpawner;
		protected DialogueSystem dialogueSystem;
		protected BossAttackSpheresEven evenSpheresAttack;
		protected BossAttackSpheresOdd oddSpheresAttack;
		protected BossAttackLaserSweep laserAttack;
		protected CancellationTokenSource tokenSource;
		
		private void Awake()
		{
			boss = GetComponent<BossBehaviour>();
			waveSpawner = FindAnyObjectByType<WaveSpawner>();
		}

		public void StartPhase()
		{
			dialogueSystem = FindAnyObjectByType<DialogueSystem>(FindObjectsInactive.Include);
			evenSpheresAttack = GetComponent<BossAttackSpheresEven>();
			oddSpheresAttack = GetComponent<BossAttackSpheresOdd>();
			laserAttack = GetComponent<BossAttackLaserSweep>();
		
			StartCoroutine(PhaseCoroutine());
		}

		protected abstract IEnumerator PhaseCoroutine();
		protected void EndPhase() => OnPhaseEnded?.Invoke();
	}
}
