using System;
using UnityEngine;

namespace Code.EnemySystem.Boss
{
	[RequireComponent(typeof(BossBehaviour))]
	public abstract class BossPhase : MonoBehaviour
	{
		public static event Action OnPhaseEnded;
		protected BossBehaviour boss;
		
		private void Awake()
		{
			boss = GetComponent<BossBehaviour>();
		}

		public abstract void StartPhase();
		protected void EndPhase() => OnPhaseEnded?.Invoke();
	}
}
