using System.Collections;
using UnityEngine;

namespace Code.EnemySystem.Boss
{
	public class BossAttackSpheresEven : BossAttackSpheres
	{
		public override IEnumerator Shoot()
		{
			bossAnimator.AnimateAttack(0);
			for (int i = 0; i < fireballsPerShot; i++)
			{
				bossAnimator.Shoot();
				//ShootSingle();
				yield return new WaitForSeconds(timeBetweenShots);
			}
		}

		private void ShootSingle()
		{
			for (int i = 0; i < nozzles.Count; i++)
			{
				if (i % 2 == 0)
				{
					SpawnFireball(nozzles[i]);
				}
			}
		}
	}
}
