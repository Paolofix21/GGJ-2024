using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Code.EnemySystem.Boss
{
	public abstract class BossAttackSpheres : BossAttack
	{
		[SerializeField] protected List<Transform> nozzles; // NON HA SENSO CHE SIANO QUI MA NON MI È RIMASTO UN NEURONE ATTIVO
		[SerializeField] protected Fireball fireballPrefab;
	
		public abstract Task Shoot();
		protected void SpawnFireball(Transform spawnPoint)
		{
			Instantiate(fireballPrefab, spawnPoint.position, spawnPoint.rotation);
		}
	}
}
