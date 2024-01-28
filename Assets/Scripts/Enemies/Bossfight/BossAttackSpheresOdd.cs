using System.Collections;
using UnityEngine;

namespace Code.EnemySystem.Boss
{
	public class BossAttackSpheresOdd : BossAttackSpheres
	{
		public override IEnumerator Shoot()
		{
			bossAnimator.AnimateAttack(0);
			for (int i = 0; i < fireballsPerShot; i++)
			{
				ShootSingle();
				yield return new WaitForSeconds(timeBetweenShots);
			}
		}

		private void ShootSingle()
		{
			bossAnimator.AnimateAttack(0);

			for (int i = 0; i < nozzles.Count; i++)
			{
				if (i % 2 != 0)
				{
					SpawnFireball(nozzles[i]);
				}
			}
		}
	}
}
