using System.Threading.Tasks;
using UnityEngine;

namespace Code.EnemySystem.Boss
{
	public class BossAttackSpheresEven : BossAttackSpheres
	{
		public override async Task Shoot()
		{
			for (int i = 0; i < fireballsPerShot; i++)
			{
				ShootSingle();
				await Task.Delay((int)(timeBetweenShots * 1000));
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
