using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Code.EnemySystem.Boss
{
	public class BossAttackLaserSweep : BossAttack
	{
		[SerializeField] private Transform spawnPositionLeft;
		[SerializeField] private Transform spawnPositionRight;
		[SerializeField] private Laser laserPrefab;
		private Laser laser;
		private bool hasFinishedSweep;
		
		
		public async Task FireLeft()
		{
			SpawnIfNeeded();
			hasFinishedSweep = false;
			StartCoroutine(Sweep(laser.transform.rotation, laser.transform.rotation)); // TODO pass correct target rotation
			await WaitUntil(() => hasFinishedSweep);
		}

		public async Task FireRight()
		{
			SpawnIfNeeded();
			hasFinishedSweep = false;
			StartCoroutine(Sweep(laser.transform.rotation, laser.transform.rotation)); // TODO pass correct target rotation
			await WaitUntil(() => hasFinishedSweep);
		}

		private void SpawnIfNeeded()
		{
			if (laser == null)
			{
				laser = Instantiate(laserPrefab);
			}
		}

		private IEnumerator Sweep(Quaternion from, Quaternion to)
		{
			laser.gameObject.SetActive(true);

			// TODO handle rotation here
			yield return null;
			
			laser.gameObject.SetActive(false);
			hasFinishedSweep = true;
		}


		private async Task WaitUntil(Func<bool> predicate)
		{
			while (!predicate.Invoke())
			{
				await Task.Yield();
			}
		}
	}
}
