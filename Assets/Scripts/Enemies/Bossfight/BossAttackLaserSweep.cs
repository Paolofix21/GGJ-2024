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
		[SerializeField] private float sweepSpeed;
		private Laser laserLeft, laserRight;
		private bool hasFinishedSweep;
		
		
		public async Task FireLeft()
		{
			float laserPrepareDuration = 5f;
			bossAnimator.AnimateAttack(1, laserPrepareDuration);
			await Task.Delay((int)(laserPrepareDuration * 1000));
			
			SpawnIfNeeded();
			hasFinishedSweep = false;
			StartCoroutine(Sweep(90));
			await WaitUntil(() => hasFinishedSweep);
		}

		public async Task FireRight()
		{
			SpawnIfNeeded();
			hasFinishedSweep = false;
			StartCoroutine(Sweep(-90));
			await WaitUntil(() => hasFinishedSweep);
		}

		private void SpawnIfNeeded()
		{
			if (laserLeft == null)
			{
				laserLeft = Instantiate(laserPrefab, spawnPositionLeft);
				laserLeft.transform.rotation = Quaternion.Euler(45f, transform.eulerAngles.y, 0f);
			}
			
			if (laserRight == null)
			{
				laserRight = Instantiate(laserPrefab, spawnPositionRight);
				laserRight.transform.rotation = Quaternion.Euler(45f, transform.eulerAngles.y, 0f);
			}
		}

		private IEnumerator Sweep(float targetSweepAngle)
		{ 
			float currentSweepAngle = 0f;

			while (currentSweepAngle > targetSweepAngle)
			{
				currentSweepAngle += sweepSpeed * Time.deltaTime;
				float lerpFactor = Mathf.Sin(Mathf.Deg2Rad * currentSweepAngle / targetSweepAngle);
				float angle = Mathf.Lerp(-targetSweepAngle / 2f, targetSweepAngle / 2f, lerpFactor);
				laserLeft.transform.localRotation = Quaternion.Euler(0f, angle, 0f);
				laserRight.transform.localRotation = Quaternion.Euler(0f, -angle, 0f);
				yield return null;
			}
			
			laserLeft.gameObject.SetActive(false);
			laserRight.gameObject.SetActive(false);
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
