using Code.Graphics;
using UnityEngine;

namespace Code.EnemySystem.Boss
{
	public abstract class BossAttack : MonoBehaviour
	{
		protected BossAnimator bossAnimator;

		protected virtual void Awake()
		{
			bossAnimator = GetComponentInChildren<BossAnimator>();
		}
	}
}
