using System.Threading.Tasks;

namespace Code.EnemySystem.Boss
{
	public class BossAttackSpheresEven : BossAttackSpheres
	{
		public override async Task Shoot()
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
