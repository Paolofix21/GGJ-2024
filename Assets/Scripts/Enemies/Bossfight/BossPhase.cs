using System;
using UnityEngine;

namespace Code.EnemySystem.Boss
{
	[RequireComponent(typeof(BossBehaviour))]
	public abstract class BossPhase : MonoBehaviour
	{
		public static event Action OnPhaseEnded;
		protected BossBehaviour boss;
		protected WaveSpawner waveSpawner;
		
		private void Awake()
		{
			boss = GetComponent<BossBehaviour>();
			waveSpawner = FindAnyObjectByType<WaveSpawner>();
		}

		public abstract void StartPhase();
		protected void EndPhase() => OnPhaseEnded?.Invoke();
	}
}
