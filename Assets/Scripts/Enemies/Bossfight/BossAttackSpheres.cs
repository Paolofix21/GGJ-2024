using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.EnemySystem.Boss
{
	public abstract class BossAttackSpheres : BossAttack
	{
		[SerializeField] protected List<Transform> nozzles;
		[SerializeField] protected Fireball fireballPrefab;
		[Space]
		[SerializeField, Min(1)] protected int fireballsPerShot = 3;
		[SerializeField, Min(0.00001f)] protected float timeBetweenShots = 1f;

		
		public abstract IEnumerator Shoot();
		
		protected void SpawnFireball(Transform spawnPoint)
		{
			Instantiate(fireballPrefab, spawnPoint.position, spawnPoint.rotation);
		}
	}
}
