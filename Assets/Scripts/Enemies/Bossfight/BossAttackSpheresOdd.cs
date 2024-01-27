using System.Threading.Tasks;

namespace Code.EnemySystem.Boss
{
	public class BossAttackSpheresOdd : BossAttackSpheres
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
				if (i % 2 != 0)
				{
					SpawnFireball(nozzles[i]);
				}
			}
		}
	}
}
